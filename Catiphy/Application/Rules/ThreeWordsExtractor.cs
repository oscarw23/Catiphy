namespace Catiphy.Application.Rules;
public static class ThreeWordsExtractor
{
    public static string FromFact(string fact)
    {
        if (string.IsNullOrWhiteSpace(fact)) return string.Empty;
        var cleaned = fact.Trim().TrimEnd('.', ',', ';', ':', '!', '?');
        var parts = System.Text.RegularExpressions.Regex.Split(cleaned, @"\s+")
                        .Where(p => !string.IsNullOrWhiteSpace(p))
                        .Take(3);
        return string.Join(' ', parts);
    }
}

