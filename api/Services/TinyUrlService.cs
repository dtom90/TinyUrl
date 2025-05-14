using TinyUrl.Api.Models;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace TinyUrl.Api.Services;

public interface ITinyUrlService
{
    Task<TinyUrlRecord> CreateTinyUrlAsync(TinyUrlRequest request);
    Task<IEnumerable<TinyUrlRecord>> GetAllUrlsAsync();
    Task<string?> GetLongUrlAsync(string id);
    Task<TinyUrlRecord?> DeleteUrlAsync(string id);
}

public class TinyUrlService : ITinyUrlService
{
    private readonly ConcurrentDictionary<string, TinyUrlRecord> _urlStore = new();
    
    // Note: in a real application, these should be set in configuration. Hard-coding them here for simplicity.
    private const string BaseUrl = "http://localhost:5226/";
    private const int MinShortCodeLength = 3;
    private const int MaxShortCodeLength = 8;
    private const int DefaultShortCodeLength = 6;
    private const int MaxGenerateShortCodeAttempts = 10;
    private static readonly string[] ReservedShortCodes = new[] { "api", "tinyurl", "tinyurls" };
    private readonly ILogger<TinyUrlService> _logger;

    public TinyUrlService(ILogger<TinyUrlService> logger)
    {
        _logger = logger;
    }

    public Task<TinyUrlRecord> CreateTinyUrlAsync(TinyUrlRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LongUrl))
        {
            throw new ArgumentException("Long URL is required");
        }

        if (!Uri.TryCreate(request.LongUrl, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException($"\"{request.LongUrl}\" is not a valid URL");
        }

        string shortCode;
        if (!string.IsNullOrWhiteSpace(request.ShortCode))
        {
            if (!Regex.IsMatch(request.ShortCode, "^[a-zA-Z0-9]+$"))
            {
                throw new ArgumentException($"Short code \"{request.ShortCode}\" is not valid (only alphanumeric characters are allowed)");
            }

            if (request.ShortCode.Length < MinShortCodeLength)
            {
                throw new ArgumentException($"Short code \"{request.ShortCode}\" is too short (minimum length is {MinShortCodeLength} characters)");
            }

            if (request.ShortCode.Length > MaxShortCodeLength)
            {
                throw new ArgumentException($"Short code \"{request.ShortCode}\" is too long (maximum length is {MaxShortCodeLength} characters)");
            }

            if (ReservedShortCodes.Contains(request.ShortCode))
            {
                throw new ArgumentException($"Short code \"{request.ShortCode}\" is reserved for internal use");
            }

            if (_urlStore.ContainsKey(request.ShortCode))
            {
                throw new ArgumentException($"Short code \"{request.ShortCode}\" is already in use");
            }

            shortCode = request.ShortCode;
        }
        else
        {
            shortCode = GenerateShortCode();
        }

        var tinyUrl = new TinyUrlRecord(
            Id: shortCode,
            LongUrl: request.LongUrl,
            ShortUrl: $"{BaseUrl}{shortCode}",
            ClickCount: 0
        );

        if (!_urlStore.TryAdd(shortCode, tinyUrl))
        {
            // handle the case where the short code we chose was added concurrently
            // we try again with a new short code
            shortCode = GenerateShortCode();
            if (!_urlStore.TryAdd(shortCode, tinyUrl))
            {
                // if we fail again, we give up
                _logger.LogError($"Failed to add to _urlStore for short code '{shortCode}' - it may have been added concurrently");
                throw new InvalidOperationException();
            }
        }
        return Task.FromResult(tinyUrl);
    }

    public Task<IEnumerable<TinyUrlRecord>> GetAllUrlsAsync()
    {
        return Task.FromResult(_urlStore.Values.AsEnumerable());
    }

    public Task<string?> GetLongUrlAsync(string id)
    {
        _urlStore.TryGetValue(id, out var tinyUrl);
        _ = UpdateClickCountAsync(id); // Asynchronously update the click count to avoid blocking the request
        return Task.FromResult(tinyUrl?.LongUrl);
    }

    public Task<TinyUrlRecord?> DeleteUrlAsync(string id)
    {
        if (_urlStore.TryGetValue(id, out var tinyUrl))
        {
            _urlStore.Remove(id, out _);
            return Task.FromResult<TinyUrlRecord?>(tinyUrl);
        }
        return Task.FromResult<TinyUrlRecord?>(null);
    }

    private string GenerateShortCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        int attempts = 0;
        string shortCode;
        do
        {
            var bytes = new byte[DefaultShortCodeLength]; // 6 bytes mapped to 62 characters gives us 62^6 ≈ 56.8 billion possible short codes
            RandomNumberGenerator.Fill(bytes);
            shortCode = new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
        } while (_urlStore.ContainsKey(shortCode) && attempts < MaxGenerateShortCodeAttempts);
        if (attempts >= MaxGenerateShortCodeAttempts)
        {
            throw new InvalidOperationException("Failed to generate a unique short code");
        }
        return shortCode;
    }

    private Task UpdateClickCountAsync(string id)
    {
        _urlStore.TryGetValue(id, out var tinyUrl);
        if (tinyUrl != null)
        {
            _urlStore[id] = tinyUrl with { ClickCount = tinyUrl.ClickCount + 1 };
        }
        return Task.CompletedTask;
    }
}
