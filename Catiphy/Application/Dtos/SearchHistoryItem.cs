namespace Catiphy.Application.Dtos
{
    public record SearchHistoryItem(
        DateTime SearchedAtUtc,
        string FactText,
        string ThreeWords,
        string GifUrl
    );
}
