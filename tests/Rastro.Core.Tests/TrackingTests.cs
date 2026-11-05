using Rastro.Core.Models;
using Rastro.Core.Services;
using Rastro.Core.ViewModels;

namespace Rastro.Core.Tests;

public sealed class TrackingTests
{
    private static DemoTrackingService Service() => new(new FixedClock(), TimeSpan.Zero);
    private static (TrackingViewModel Vm, MemoryStore Store, TestNavigation Navigation) Create(ITrackingService? service = null)
    {
        var store = new MemoryStore();
        var navigation = new TestNavigation();
        return (new(service ?? Service(), store, navigation), store, navigation);
    }

    [Theory]
    [InlineData("BR123456789SP", true)]
    [InlineData(" br123456789sp ", true)]
    [InlineData("", false)]
    [InlineData("BR123SP", false)]
    [InlineData("BR１２３４５６７８９SP", false)]
    [InlineData("BR123456789SP\nINVALID", false)]
    public void ValidatesTrackingFormat(string code, bool valid) => Assert.Equal(valid, TrackingCode.IsValid(code));

    [Theory]
    [InlineData("BR123456789SP", PackageStatus.InTransit)]
    [InlineData("BR987654321SP", PackageStatus.OutForDelivery)]
    [InlineData("BR456789123SP", PackageStatus.Delivered)]
    [InlineData("BR111222333SP", PackageStatus.Posted)]
    public async Task FindsAllDemoStatusesWithConsistentDates(string code, PackageStatus status)
    {
        var package = await Service().FindAsync(code);
        Assert.NotNull(package);
        Assert.Equal(status, package.Status);
        Assert.True(package.ExpectedDelivery >= package.ShippedAt);
        Assert.True(package.Events.SequenceEqual(package.Events.OrderByDescending(e => e.Date)));
        Assert.All(package.Events, e => Assert.True(e.Date <= new FixedClock().GetUtcNow()));
        Assert.Equal(package.CurrentLocation, package.Events[0].Location);
    }

    [Theory]
    [InlineData("", "Digite")]
    [InlineData("invalid", "formato")]
    [InlineData("BR000000000SP", "não encontrada")]
    public async Task InvalidOrMissingPackageDoesNotNavigate(string code, string expected)
    {
        var (vm, _, nav) = Create();
        await vm.TrackAsync(code);
        Assert.Contains(expected, vm.ErrorMessage);
        Assert.Null(nav.Opened);
        Assert.False(vm.IsBusy);
    }

    [Fact]
    public async Task ValidSearchNormalizesNavigatesAndPersistsRecentCodes()
    {
        var (vm, store, nav) = Create();
        await vm.InitializeAsync();
        await vm.TrackAsync(" br123456789sp ");
        Assert.Equal("BR123456789SP", nav.Opened?.Id);
        Assert.Equal("BR123456789SP", store.Value.RecentCodes.Single());
        await vm.TrackAsync("BR123456789SP");
        Assert.Single(vm.RecentSearches);
        Assert.False(vm.HasError);
    }

    [Fact]
    public async Task FiltersAndStatisticsReflectSavedPackages()
    {
        var (vm, _, _) = Create();
        await vm.InitializeAsync();
        Assert.Equal(3, vm.SavedCount);
        Assert.Equal(2, vm.ActiveCount);
        Assert.Equal(1, vm.DeliveredCount);
        await vm.FilterCommand.ExecuteAsync("Entregues");
        Assert.Equal(PackageStatus.Delivered, Assert.Single(vm.Packages).Status);
        await vm.FilterCommand.ExecuteAsync("Em andamento");
        Assert.Equal(2, vm.Packages.Count);
        Assert.DoesNotContain(vm.Packages, p => p.IsDelivered);
    }

    [Fact]
    public async Task SaveAndUnsavePersistAcrossViewModels()
    {
        var (vm, store, nav) = Create();
        await vm.InitializeAsync();
        await vm.TrackAsync("BR111222333SP");
        Assert.False(vm.IsSaved);
        await vm.SaveCommand.ExecuteAsync();
        Assert.True(vm.IsSaved);
        var next = new TrackingViewModel(Service(), store, nav);
        await next.InitializeAsync();
        Assert.Equal(4, next.SavedCount);
        await vm.SaveCommand.ExecuteAsync();
        Assert.False(vm.IsSaved);
        Assert.Equal(3, vm.SavedCount);
    }

    [Fact]
    public async Task RemovingEveryPackageShowsEmptyState()
    {
        var (vm, _, _) = Create();
        await vm.InitializeAsync();
        foreach (var code in TrackingPreferences.Default.SavedCodes)
        {
            await vm.TrackAsync(code);
            await vm.SaveCommand.ExecuteAsync();
        }
        Assert.True(vm.HasNoPackages);
        Assert.Equal(0, vm.SavedCount);
    }

    [Fact]
    public async Task ServiceFailureRestoresControlsAndCanBeRetried()
    {
        var (vm, _, _) = Create(new FailingService());
        await vm.TrackAsync("BR123456789SP");
        Assert.True(vm.HasError);
        Assert.False(vm.IsBusy);
        vm.Code = "BR987654321SP";
        Assert.False(vm.HasError);
    }

    [Fact]
    public async Task DuplicateSearchDoesNotStartSecondRequest()
    {
        var slow = new DeferredService();
        var (vm, _, _) = Create(slow);
        var first = vm.TrackAsync("BR123456789SP");
        Assert.True(vm.IsBusy);
        await vm.TrackAsync("BR987654321SP");
        Assert.Equal(1, slow.Calls);
        slow.Completion.SetResult(Service().DemoPackages[0]);
        await first;
        Assert.False(vm.IsBusy);
    }

    [Fact]
    public async Task RefreshAndCopyActOnCurrentPackage()
    {
        var (vm, _, nav) = Create();
        await vm.TrackAsync("BR123456789SP");
        var package = vm.Package;
        await vm.RefreshCommand.ExecuteAsync();
        Assert.Equal(package, vm.Package);
        Assert.True(vm.HasNotice);
        await vm.CopyCommand.ExecuteAsync();
        Assert.Equal(package!.Id, nav.Copied);
    }

    [Fact]
    public async Task DiskFailureDoesNotBlockViewingPackage()
    {
        var nav = new TestNavigation();
        var vm = new TrackingViewModel(Service(), new FailingStore(), nav);
        await vm.TrackAsync("BR123456789SP");
        Assert.NotNull(nav.Opened);
        Assert.Contains("salvar", vm.ErrorMessage);
        Assert.False(vm.IsBusy);
    }

    [Fact]
    public async Task JsonStoreRoundTripsAndRecoversFromCorruption()
    {
        var directory = Path.Combine(Path.GetTempPath(), "rastro-test-" + Guid.NewGuid());
        var path = Path.Combine(directory, "preferences.json");
        try
        {
            var store = new JsonTrackingStore(path);
            Assert.Equal(3, (await store.LoadAsync()).SavedCodes.Length);
            var preferences = new TrackingPreferences([], ["BR123456789SP"]);
            await store.SaveAsync(preferences);
            var loaded = await store.LoadAsync();
            Assert.Empty(loaded.SavedCodes);
            Assert.Equal(preferences.RecentCodes, loaded.RecentCodes);
            await File.WriteAllTextAsync(path, "{invalid");
            Assert.Equal(3, (await store.LoadAsync()).SavedCodes.Length);
            await File.WriteAllTextAsync(path, "{\"SavedCodes\":null,\"RecentCodes\":null}");
            Assert.Equal(3, (await store.LoadAsync()).SavedCodes.Length);
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }

    [Fact]
    public async Task CommandPreventsReentryAndReportsErrors()
    {
        var completion = new TaskCompletionSource();
        var calls = 0;
        Exception? error = null;
        var command = new AsyncCommand(async _ => { calls++; await completion.Task; throw new IOException(); }, ex => error = ex);
        var first = command.ExecuteAsync();
        Assert.False(command.CanExecute(null));
        await command.ExecuteAsync();
        Assert.Equal(1, calls);
        completion.SetResult();
        await first;
        Assert.IsType<IOException>(error);
        Assert.True(command.CanExecute(null));
    }

    private sealed class FixedClock : TimeProvider
    { public override DateTimeOffset GetUtcNow() => new(2026, 11, 5, 14, 0, 0, TimeSpan.Zero); }
    private sealed class MemoryStore : ITrackingStore
    {
        public TrackingPreferences Value = TrackingPreferences.Default;
        public Task<TrackingPreferences> LoadAsync() => Task.FromResult(Value);
        public Task SaveAsync(TrackingPreferences value) { Value = value; return Task.CompletedTask; }
    }
    private sealed class TestNavigation : ITrackingNavigation
    {
        public Package? Opened;
        public string? Copied;
        public Task ShowPackageAsync(Package package) { Opened = package; return Task.CompletedTask; }
        public Task GoHomeAsync() => Task.CompletedTask;
        public Task ShowHelpAsync() => Task.CompletedTask;
        public Task CopyCodeAsync(string code) { Copied = code; return Task.CompletedTask; }
    }
    private sealed class FailingService : ITrackingService
    {
        public IReadOnlyList<Package> DemoPackages => [];
        public Task<Package?> FindAsync(string code, CancellationToken cancellationToken = default) => throw new IOException();
    }
    private sealed class DeferredService : ITrackingService
    {
        public int Calls;
        public TaskCompletionSource<Package?> Completion { get; } = new();
        public IReadOnlyList<Package> DemoPackages => [];
        public Task<Package?> FindAsync(string code, CancellationToken cancellationToken = default) { Calls++; return Completion.Task; }
    }
    private sealed class FailingStore : ITrackingStore
    {
        public Task<TrackingPreferences> LoadAsync() => Task.FromResult(TrackingPreferences.Default);
        public Task SaveAsync(TrackingPreferences preferences) => throw new IOException();
    }
}
