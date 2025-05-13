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
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RedirectToLongUrl(string id)
    {
        var tinyUrl = await _tinyUrlService.GetUrlAsync(id);
        if (tinyUrl == null)
        {
            _logger.LogWarning("Tiny URL not found: {Id}", id);
            return NotFound();
        }

        _logger.LogInformation("Redirecting {Id} to {LongUrl}", id, tinyUrl.LongUrl);
        return Redirect(tinyUrl.LongUrl);
    }
}
