using Catiphy.Infrastructure.Clients;
using Catiphy.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public sealed class GifController(IGiphyClient giphy, ISearchHistoryRepository repo) : ControllerBase
{
    // GET /api/gif?query=...&fact=...&prev=...
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string query, [FromQuery] string? fact = null, [FromQuery] string? prev = null)
    {
        if (string.IsNullOrWhiteSpace(query)) return BadRequest("query is required");

        var gifUrl = await giphy.SearchRandomGifUrlAsync(query, prev);
        if (string.IsNullOrWhiteSpace(gifUrl)) return NoContent();

        if (!string.IsNullOrWhiteSpace(fact))
            await repo.InsertAsync(DateTime.UtcNow, fact, query, gifUrl);

        return Ok(new { gifUrl });
    }
}
