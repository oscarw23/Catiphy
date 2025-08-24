namespace Catiphy.Infrastructure.Clients;
public record CatFactDto(string fact, int length);

public interface ICatFactsClient
{
    Task<CatFactDto?> GetRandomAsync();
}

public sealed class CatFactsClient(HttpClient http) : ICatFactsClient
{
    public async Task<CatFactDto?> GetRandomAsync()
        => await http.GetFromJsonAsync<CatFactDto>("fact");
}
