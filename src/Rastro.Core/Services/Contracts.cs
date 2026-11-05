using Rastro.Core.Models;

namespace Rastro.Core.Services;

public interface ITrackingService
{
    IReadOnlyList<Package> DemoPackages { get; }
    Task<Package?> FindAsync(string code, CancellationToken cancellationToken = default);
}

public sealed record TrackingPreferences(string[] SavedCodes, string[] RecentCodes)
{
    public static TrackingPreferences Default => new(
        ["BR123456789SP", "BR987654321SP", "BR456789123SP"], []);
}

public interface ITrackingStore
{
    Task<TrackingPreferences> LoadAsync();
    Task SaveAsync(TrackingPreferences preferences);
}

public interface ITrackingNavigation
{
    Task ShowPackageAsync(Package package);
    Task GoHomeAsync();
    Task ShowHelpAsync();
    Task CopyCodeAsync(string code);
}
