using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CatiphyWeb.Dtos;

namespace CatiphyWeb.Services;
public class CatiphyApi
{
    private readonly HttpClient _http;
    public CatiphyApi(HttpClient http) => _http = http;

    public Task<FactDto?> GetFactAsync()
        => _http.GetFromJsonAsync<FactDto>("api/fact");

    public async Task<string?> GetGifUrlAsync(string query, string? fact = null, string? prev = null)
    {
        var url = $"api/gif?query={Uri.EscapeDataString(query)}";
        if (!string.IsNullOrWhiteSpace(fact)) url += $"&fact={Uri.EscapeDataString(fact)}";
        if (!string.IsNullOrWhiteSpace(prev)) url += $"&prev={Uri.EscapeDataString(prev)}";

        var resp = await _http.GetAsync(url);
        if (resp.StatusCode == HttpStatusCode.NoContent) return null;
        if (!resp.IsSuccessStatusCode) return null;

        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
        return json.TryGetProperty("gifUrl", out var p) ? p.GetString() : null;
    }


    public async Task<(List<HistoryItemDto> items, int total)?> GetHistoryAsync(
        int skip,
        int take, 
        string? q = null,
        DateTime? FromDate = null, 
        DateTime? ToDate = null)
    {
        var qs = new List<string> { $"skip={skip}", $"take={take}" };
        if (!string.IsNullOrWhiteSpace(q)) qs.Add($"q={Uri.EscapeDataString(q)}");
        if (FromDate is not null) qs.Add($"from={Uri.EscapeDataString(FromDate.Value.ToString("o"))}");
        if (ToDate is not null) qs.Add($"to={Uri.EscapeDataString(ToDate.Value.ToString("o"))}");

        var url = "api/history" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");
        var resp = await _http.GetAsync(url);
        if (!resp.IsSuccessStatusCode) return (new List<HistoryItemDto>(), 0);

        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();

        var list = new List<HistoryItemDto>();
        if (json.TryGetProperty("items", out var itemsProp) && itemsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var el in itemsProp.EnumerateArray())
            {
                DateTime searchedAt = default;
                if (el.TryGetProperty("searchedAtUtc", out var d) && d.ValueKind == JsonValueKind.String)
                    searchedAt = DateTime.Parse(d.GetString()!);
                else if (d.ValueKind == JsonValueKind.Number)
                    searchedAt = d.GetDateTime();

                var fact = el.TryGetProperty("factText", out var f) ? (f.GetString() ?? "") : "";
                var three = el.TryGetProperty("threeWords", out var t) ? (t.GetString() ?? "") : "";
                var gifUrl = el.TryGetProperty("gifUrl", out var g) ? (g.GetString() ?? "") : "";

                list.Add(new HistoryItemDto(searchedAt, fact, three, gifUrl)); // record posicional
            }
        }

        var total = json.TryGetProperty("total", out var tot) && tot.ValueKind is JsonValueKind.Number
            ? tot.GetInt32()
            : list.Count;

        return (list, total);
    }



    public string GetExportUrl(string fmt = "csv")
    {
        var baseUri = _http.BaseAddress ?? throw new InvalidOperationException("HttpClient.BaseAddress not set");
        return new Uri(baseUri, "api/history/export").ToString();
    }
}
