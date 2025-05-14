using Microsoft.AspNetCore.Mvc;
using TinyUrl.Api.Models;
using TinyUrl.Api.Services;

namespace TinyUrl.Api.Controllers;

[ApiController]
[Route("api/tinyurls")]
public class TinyUrlController : ControllerBase
{
    private readonly ITinyUrlService _tinyUrlService;
    private readonly ILogger<TinyUrlController> _logger;

    public TinyUrlController(ITinyUrlService tinyUrlService, ILogger<TinyUrlController> logger)
    {
        _tinyUrlService = tinyUrlService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TinyUrlRecord>> CreateTinyUrl(TinyUrlRequest request)
    {
        try
        {
            var tinyUrl = await _tinyUrlService.CreateTinyUrlAsync(request);
            return CreatedAtAction(
                nameof(GetTinyUrl),
                new { id = tinyUrl.Id },
                tinyUrl);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request to create tiny URL");
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TinyUrlRecord>>> GetAllUrls()
    {
        var urls = await _tinyUrlService.GetAllUrlsAsync();
        return Ok(urls);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TinyUrlRecord>> GetTinyUrl(string id)
    {
        var tinyUrl = await _tinyUrlService.GetLongUrlAsync(id);
        if (tinyUrl == null)
        {
            return NotFound(new ErrorResponse($"Tiny URL with id '{id}' not found"));
        }
        return Ok(tinyUrl);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTinyUrl(string id)
    {
        var tinyUrl = await _tinyUrlService.DeleteUrlAsync(id);
        if (tinyUrl == null)
        {
            return NotFound(new ErrorResponse($"Tiny URL with id '{id}' not found"));
        }
        return Ok(tinyUrl);
    }
}
