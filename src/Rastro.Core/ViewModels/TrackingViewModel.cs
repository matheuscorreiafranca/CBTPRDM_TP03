using System.Collections.ObjectModel;
using Rastro.Core.Models;
using Rastro.Core.Services;

namespace Rastro.Core.ViewModels;

// Dupla: Matheus Correia França e Davi Leite Coelho — CB303142X.
// A lógica de rastreamento depende de contratos e não conhece controles XAML.
public sealed class TrackingViewModel : ObservableObject
{
    private readonly ITrackingService _service;
    private readonly ITrackingStore _store;
    private readonly ITrackingNavigation _navigation;
    private TrackingPreferences _preferences = TrackingPreferences.Default;
    private string _code = "", _error = "", _filter = "Todas", _notice = "";
    private bool _busy, _initialized;
    private Package? _package;
    private DateTimeOffset _updatedAt;

    public TrackingViewModel(ITrackingService service, ITrackingStore store, ITrackingNavigation navigation)
    {
        (_service, _store, _navigation) = (service, store, navigation);
        TrackCommand = Command(async value => await TrackAsync(value as string ?? Code));
        FilterCommand = Command(value => { Filter = value as string ?? "Todas"; RebuildPackages(); return Task.CompletedTask; });
        HomeCommand = Command(async _ => { ErrorMessage = ""; await _navigation.GoHomeAsync(); });
        SavedCommand = Command(async _ => { Filter = "Todas"; RebuildPackages(); await _navigation.GoHomeAsync(); });
        HelpCommand = Command(_ => _navigation.ShowHelpAsync());
        RefreshCommand = Command(_ => RefreshAsync());
        SaveCommand = Command(_ => ToggleSavedAsync());
        CopyCommand = Command(async _ =>
        {
            if (Package is null) return;
            await _navigation.CopyCodeAsync(Package.Id);
            Notice = "Código copiado. Pronto para compartilhar!";
        });
        RebuildPackages();
    }

    public string Code { get => _code; set { if (Set(ref _code, value)) ErrorMessage = ""; } }
    public string ErrorMessage { get => _error; private set { Set(ref _error, value); Notify(nameof(HasError)); } }
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public string Notice { get => _notice; private set { Set(ref _notice, value); Notify(nameof(HasNotice)); } }
    public bool HasNotice => !string.IsNullOrEmpty(Notice);
    public bool IsBusy { get => _busy; private set { Set(ref _busy, value); Notify(nameof(IsReady)); Notify(nameof(TrackButtonText)); } }
    public bool IsReady => !IsBusy;
    public string TrackButtonText => IsBusy ? "Buscando…" : "Rastrear encomenda  →";
    public Package? Package { get => _package; private set { Set(ref _package, value); Notify(nameof(IsSaved)); Notify(nameof(SaveButtonText)); } }
    public string Filter { get => _filter; private set { Set(ref _filter, value); Notify(nameof(AllSelected)); Notify(nameof(ActiveSelected)); Notify(nameof(DeliveredSelected)); } }
    public bool AllSelected => Filter == "Todas";
    public bool ActiveSelected => Filter == "Em andamento";
    public bool DeliveredSelected => Filter == "Entregues";
    public bool IsSaved => Package is not null && _preferences.SavedCodes.Contains(Package.Id);
    public string SaveButtonText => IsSaved ? "✓  Encomenda salva" : "+  Salvar encomenda";
    public string UpdatedLabel => $"Atualizado às {_updatedAt:HH:mm:ss} · dados simulados";
    public int SavedCount => _preferences.SavedCodes.Length;
    public int ActiveCount => SavedPackages.Count(p => p.Status != PackageStatus.Delivered);
    public int DeliveredCount => SavedPackages.Count(p => p.Status == PackageStatus.Delivered);
    public string PackageCountLabel => $"{Packages.Count} {(Packages.Count == 1 ? "encomenda" : "encomendas")}";
    public bool HasNoPackages => Packages.Count == 0;
    public bool HasRecentSearches => RecentSearches.Count > 0;
    public ObservableCollection<Package> Packages { get; } = [];
    public ObservableCollection<string> RecentSearches { get; } = [];
    public AsyncCommand TrackCommand { get; }
    public AsyncCommand FilterCommand { get; }
    public AsyncCommand HomeCommand { get; }
    public AsyncCommand SavedCommand { get; }
    public AsyncCommand HelpCommand { get; }
    public AsyncCommand RefreshCommand { get; }
    public AsyncCommand SaveCommand { get; }
    public AsyncCommand CopyCommand { get; }
    private IEnumerable<Package> SavedPackages => _service.DemoPackages.Where(p => _preferences.SavedCodes.Contains(p.Id));
    private AsyncCommand Command(Func<object?, Task> action) => new(action, _ => ErrorMessage = "Não foi possível concluir agora. Tente novamente.");

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        _preferences = await _store.LoadAsync();
        _initialized = true;
        RebuildPackages();
        RebuildRecent();
    }

    public async Task TrackAsync(string code)
    {
        if (IsBusy) return;
        ErrorMessage = "";
        Notice = "";
        code = TrackingCode.Normalize(code);
        if (code.Length == 0) { ErrorMessage = "Digite o código da encomenda para continuar."; return; }
        if (!TrackingCode.IsValid(code))
        {
            ErrorMessage = "Confira o formato: 2 letras, 9 números e 2 letras. Ex.: BR123456789SP.";
            return;
        }
        IsBusy = true;
        try
        {
            var result = await _service.FindAsync(code);
            if (result is null)
            {
                ErrorMessage = "Encomenda não encontrada. Confira o código ou experimente BR123456789SP.";
                return;
            }
            Package = result;
            _updatedAt = DateTimeOffset.Now;
            Notify(nameof(UpdatedLabel));
            _preferences = _preferences with { RecentCodes = new[] { code }.Concat(_preferences.RecentCodes).Distinct().Take(5).ToArray() };
            await PersistAsync();
            RebuildRecent();
            await _navigation.ShowPackageAsync(result);
        }
        catch (Exception) { ErrorMessage = "Não conseguimos consultar a encomenda. Tente novamente."; }
        finally { IsBusy = false; }
    }

    private async Task RefreshAsync()
    {
        if (Package is null || IsBusy) return;
        IsBusy = true;
        ErrorMessage = "";
        Notice = "";
        try
        {
            var updated = await _service.FindAsync(Package.Id);
            if (updated is null) { ErrorMessage = "A encomenda não está disponível agora. Tente novamente."; return; }
            Package = updated;
            _updatedAt = DateTimeOffset.Now;
            Notify(nameof(UpdatedLabel));
            Notice = "Consulta atualizada. Você está vendo o último status da demonstração.";
        }
        catch (Exception) { ErrorMessage = "Não foi possível atualizar. Seus dados anteriores continuam disponíveis."; }
        finally { IsBusy = false; }
    }

    private async Task ToggleSavedAsync()
    {
        if (Package is null) return;
        var wasSaved = IsSaved;
        _preferences = _preferences with
        {
            SavedCodes = wasSaved ? _preferences.SavedCodes.Where(c => c != Package.Id).ToArray()
                : _preferences.SavedCodes.Append(Package.Id).ToArray()
        };
        await PersistAsync();
        Notify(nameof(IsSaved));
        Notify(nameof(SaveButtonText));
        Notice = wasSaved ? "Encomenda removida das suas entregas salvas." : "Encomenda salva. Acompanhe pela página inicial.";
        RebuildPackages();
    }

    private async Task PersistAsync()
    {
        try { await _store.SaveAsync(_preferences); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        { ErrorMessage = "Consulta disponível, mas não foi possível salvar no dispositivo."; }
    }

    private void RebuildPackages()
    {
        var filtered = SavedPackages.Where(p => Filter switch
        {
            "Em andamento" => p.Status != PackageStatus.Delivered,
            "Entregues" => p.Status == PackageStatus.Delivered,
            _ => true
        });
        Packages.Clear();
        foreach (var item in filtered) Packages.Add(item);
        foreach (var name in new[] { nameof(SavedCount), nameof(ActiveCount), nameof(DeliveredCount), nameof(PackageCountLabel), nameof(HasNoPackages) }) Notify(name);
    }

    private void RebuildRecent()
    {
        RecentSearches.Clear();
        foreach (var code in _preferences.RecentCodes) RecentSearches.Add(code);
        Notify(nameof(HasRecentSearches));
    }
}
