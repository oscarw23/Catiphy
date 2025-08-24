namespace CatiphyWeb.Dtos
{
    public record HistoryItemDto(DateTime searchedAtUtc, string factText, string threeWords, string gifUrl);
}
