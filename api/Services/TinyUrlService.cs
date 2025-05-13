using TinyUrl.Api.Models;

namespace TinyUrl.Api.Services;

public class TinyUrlService : ITinyUrlService
{
    private readonly Dictionary<string, TinyUrlRecord> _urlStore = new();
    private const string BaseUrl = "https://tinyurl.com/";
    private readonly Random _random = new();

    public Task<TinyUrlRecord> CreateTinyUrlAsync(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            throw new ArgumentException("Long URL is required", nameof(longUrl));
        }

        string shortCode;
        do
        {
            shortCode = GenerateShortCode();
        } while (_urlStore.ContainsKey(shortCode));

        var tinyUrl = new TinyUrlRecord(
            Id: shortCode,
            LongUrl: longUrl,
            ShortUrl: $"{BaseUrl}{shortCode}"
        );

        _urlStore[shortCode] = tinyUrl;
        return Task.FromResult(tinyUrl);
    }

    public Task<IEnumerable<TinyUrlRecord>> GetAllUrlsAsync()
    {
        return Task.FromResult(_urlStore.Values.AsEnumerable());
    }

    public Task<TinyUrlRecord?> DeleteUrlAsync(string id)
    {
        if (_urlStore.TryGetValue(id, out var tinyUrl))
        {
            _urlStore.Remove(id);
            return Task.FromResult<TinyUrlRecord?>(tinyUrl);
        }
        return Task.FromResult<TinyUrlRecord?>(null);
    }

    public Task<TinyUrlRecord?> GetUrlAsync(string id)
    {
        _urlStore.TryGetValue(id, out var tinyUrl);
        return Task.FromResult(tinyUrl);
    }

    private string GenerateShortCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}
