namespace Catiphy.Application.Interfaces
{
    public interface IGiphyClient
    {
        Task<string?> SearchRandomGifUrlAsync(string query, string? excludeUrl = null);
    }
}
