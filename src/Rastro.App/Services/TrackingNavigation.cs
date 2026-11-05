using Rastro.App.Views;
using Rastro.Core.Models;
using Rastro.Core.Services;

namespace Rastro.App.Services;

public sealed class TrackingNavigation(IServiceProvider services) : ITrackingNavigation
{
    public INavigation? Navigator { get; set; }
    public async Task ShowPackageAsync(Package package)
    {
        if (Navigator is null) throw new InvalidOperationException("Navegação ainda não inicializada.");
        if (Navigator.NavigationStack.LastOrDefault() is ResultsPage) return;
        await Navigator.PushAsync(services.GetRequiredService<ResultsPage>(), false);
    }
    public async Task GoHomeAsync() { if (Navigator is not null) await Navigator.PopToRootAsync(false); }
    public Task CopyCodeAsync(string code) => Clipboard.Default.SetTextAsync(code);
    public async Task ShowHelpAsync()
    {
        var page = Navigator?.NavigationStack.LastOrDefault();
        if (page is null) return;
        await page.DisplayAlertAsync("Um caminho simples até sua encomenda",
            "Digite um dos códigos de demonstração:\n\nBR123456789SP · Em trânsito\nBR987654321SP · Saiu para entrega\nBR456789123SP · Entregue\nBR111222333SP · Postado\n\nNa tela de detalhes, salve a encomenda para encontrá-la no início. Atualizar consulta novamente a fonte simulada; os status não mudam automaticamente.\n\nEste projeto acadêmico usa dados fictícios e não consulta transportadoras.", "Entendi");
    }
}
