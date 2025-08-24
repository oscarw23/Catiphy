using Catiphy.Application.Dtos;
using Catiphy.Infrastructure.Repositories;  
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Catiphy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistoryController : ControllerBase
{
    private readonly ISearchHistoryRepository _repo;

    // Límite defensivo para el tamaño de página
    private const int MaxTake = 100;

    public HistoryController(ISearchHistoryRepository repo)
    {
        _repo = repo;
    }




    [HttpGet]   // Devuelve el historial de búsquedas paginado y ordenado por fecha DESC.
    public async Task<ActionResult<object>> Get([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        if (skip < 0) skip = 0;
        if (take <= 0) take = 10;
        if (take > MaxTake) take = MaxTake;

        var (items, total) = await _repo.GetPagedAsync(skip, take);
        return Ok(new
        {
            items,
            total
        });
    }
    [HttpGet("export")]// Exporta la tabla historial
    public async Task<IActionResult> ExportToCsv()
    {
        var all = await _repo.GetAllAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Fecha,Cat Fact,3 palabras,URL GIF");

        foreach (var x in all)
        {
            sb.Append(ExtporToCsv(x.Fecha.ToString("yyyy-MM-dd HH:mm:ss"))).Append(';');
            sb.Append(ExtporToCsv(x.FactText)).Append(';');
            sb.Append(ExtporToCsv(x.ThreeWords)).Append(';');
            sb.Append(ExtporToCsv(x.GifUrl)).AppendLine();
        }

        // UTF-8 con BOM para que Excel en Windows abra bien los acentos
        var utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        var bytes = utf8.GetBytes(sb.ToString());
        return File(bytes, "text/csv; charset=utf-8", "HistorialCatiphy.csv");
    }

    private static string ExtporToCsv(string? s)
    {
        s ??= string.Empty;
        bool needsQuotes = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
        if (s.Contains('"')) s = s.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{s}\"" : s;
    }


}
