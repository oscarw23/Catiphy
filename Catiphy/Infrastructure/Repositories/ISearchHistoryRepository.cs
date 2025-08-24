using Catiphy.Application.Dtos;

namespace Catiphy.Infrastructure.Repositories
{
    public interface ISearchHistoryRepository
    {
        Task InsertAsync(DateTime searchedAtUtc, string factText, string threeWords, string gifUrl);
        Task<(IEnumerable<SearchHistoryItem> Items, int Total)> GetPagedAsync(int skip, int take);
        Task<IReadOnlyList<SearchHistoryItem>> GetAllAsync();
    }
}
