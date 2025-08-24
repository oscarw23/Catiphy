using Catiphy.Application.Interfaces; 
using System.Text.Json;


namespace Catiphy.Infrastructure.Clients;


public sealed class GiphyClient : IGiphyClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public GiphyClient(HttpClient http, IConfiguration cfg)
    {
        _http = http;
        _apiKey = cfg["GIPHY_API_KEY"] ?? cfg["Giphy:ApiKey"]
                  ?? throw new InvalidOperationException("GIPHY_API_KEY is missing.");
    }

    public async Task<string?> SearchRandomGifUrlAsync(string query, string? excludeUrl = null)
    {
        // hasta 3 intentos para no repetir
        for (int attempt = 0; attempt < 3; attempt++)
        {
            var offset = Random.Shared.Next(0, 50); 
            var url = $"v1/gifs/search?api_key={Uri.EscapeDataString(_apiKey)}&q={Uri.EscapeDataString(query)}&limit=1&offset={offset}&rating=g";

            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
            if (json.TryGetProperty("data", out var data) &&
                data.ValueKind == JsonValueKind.Array &&
                data.GetArrayLength() > 0)
            {
                var first = data[0];
                if (first.TryGetProperty("images", out var images) &&
                    images.TryGetProperty("fixed_height_small", out var fixedSmall) &&
                    fixedSmall.TryGetProperty("url", out var urlProp))
                {
                    var gifUrl = urlProp.GetString();
                    if (string.IsNullOrWhiteSpace(gifUrl)) return null;

                    if (excludeUrl is null || !gifUrl.Equals(excludeUrl, StringComparison.OrdinalIgnoreCase))
                        return gifUrl; 
                }
            }
        }
        return null;
    }
}
