using Catiphy.Application.Dtos;

namespace Catiphy.Infrastructure.Repositories
{
    public interface ISearchHistoryRepository
    {
        Task InsertAsync(DateTime Fecha, string factText, string threeWords, string gifUrl);
        Task<(IEnumerable<SearchHistoryItemDto> Items, int Total)> GetPagedAsync(int skip, int take);
        Task<IReadOnlyList<SearchHistoryItemDto>> GetAllAsync();
    }
}
