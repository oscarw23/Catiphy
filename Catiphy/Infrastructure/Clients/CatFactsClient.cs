using Catiphy.Application.Dtos;
using Catiphy.Application.Interfaces;
namespace Catiphy.Infrastructure.Clients;



public sealed class CatFactsClient(HttpClient http) : ICatFactsClient
{
    public async Task<CatFactDto?> GetRandomAsync()
        => await http.GetFromJsonAsync<CatFactDto>("fact");
}
