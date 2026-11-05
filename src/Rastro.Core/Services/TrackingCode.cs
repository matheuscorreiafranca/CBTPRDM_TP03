using System.Text.RegularExpressions;

namespace Rastro.Core.Services;

public static partial class TrackingCode
{
    public static string Normalize(string? code) => (code ?? "").Trim().ToUpperInvariant();
    public static bool IsValid(string? code) => CodePattern().IsMatch(Normalize(code));

    [GeneratedRegex("^[A-Z]{2}[0-9]{9}[A-Z]{2}$", RegexOptions.CultureInvariant)]
    private static partial Regex CodePattern();
}
