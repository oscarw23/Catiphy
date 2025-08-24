using Catiphy.Application.Dtos;

namespace Catiphy.Application.Interfaces
{
    public interface ICatFactsClient
    {
        Task<CatFactDto?> GetRandomAsync();
    }
}
