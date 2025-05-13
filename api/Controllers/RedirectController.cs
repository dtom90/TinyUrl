using Microsoft.AspNetCore.Mvc;
using TinyUrl.Api.Services;

namespace TinyUrl.Api.Controllers;

[ApiController]
[Route("")]
public class RedirectController : ControllerBase
{
    private readonly ITinyUrlService _tinyUrlService;
    private readonly ILogger<RedirectController> _logger;

    public RedirectController(ITinyUrlService tinyUrlService, ILogger<RedirectController> logger)
    {
        _tinyUrlService = tinyUrlService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status302Found)] // TODO: decide if we want to use 301 or 302
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RedirectToLongUrl(string id)
    {
        var longUrl = await _tinyUrlService.GetLongUrlAsync(id);
        if (longUrl == null)
        {
            return NotFound();
        }
        return Redirect(longUrl);
    }
}
