namespace Catiphy.Application.Dtos
{
    public record SearchHistoryItemDto(
        DateTime Fecha,
        string FactText,
        string ThreeWords,
        string GifUrl
    );
}
