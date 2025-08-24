namespace Catiphy.Application.Dtos
{
    public record SearchHistoryItemDtoDto(
        DateTime SearchedAtUtc,
        string FactText,
        string ThreeWords,
        string GifUrl
    );
}
