using System.Text.Json;

namespace Rastro.Core.Services;

/// <summary>Persistência local atômica. Um arquivo corrompido não impede abrir o app.</summary>
public sealed class JsonTrackingStore(string path) : ITrackingStore
{
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<TrackingPreferences> LoadAsync()
    {
        await _gate.WaitAsync();
        try
        {
            if (!File.Exists(path)) return TrackingPreferences.Default;
            await using var stream = File.OpenRead(path);
            var data = await JsonSerializer.DeserializeAsync<TrackingPreferences>(stream);
            if (data?.SavedCodes is null || data.RecentCodes is null) return TrackingPreferences.Default;
            return new(data.SavedCodes.Where(TrackingCode.IsValid).Distinct().ToArray(),
                data.RecentCodes.Where(TrackingCode.IsValid).Distinct().Take(5).ToArray());
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            return TrackingPreferences.Default;
        }
        finally { _gate.Release(); }
    }

    public async Task SaveAsync(TrackingPreferences preferences)
    {
        await _gate.WaitAsync();
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
            var temporary = path + ".tmp";
            await File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(preferences));
            File.Move(temporary, path, overwrite: true);
        }
        finally { _gate.Release(); }
    }
}
