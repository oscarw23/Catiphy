using Catiphy.Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;

namespace Catiphy.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FactController(ICatFactsClient catFacts) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var fact = await catFacts.GetRandomAsync();
        return fact is null ? NotFound() : Ok(fact);
    }
}
