using TinyUrl.Api.Models;
using System.Collections.Concurrent;

namespace TinyUrl.Api.Services;

public interface ITinyUrlService
{
    Task<TinyUrlRecord> CreateTinyUrlAsync(string longUrl);
    Task<IEnumerable<TinyUrlRecord>> GetAllUrlsAsync();
    Task<string?> GetLongUrlAsync(string id);
    Task<TinyUrlRecord?> DeleteUrlAsync(string id);
}

public class TinyUrlService : ITinyUrlService
{
    // This dictionary stores all the data related to the tiny urls, including the click count.
    // It will experience more frequent writes, due to the need to update the click count.
    private readonly ConcurrentDictionary<string, TinyUrlRecord> _urlStore = new();
    
    // This dictionary is used for quick lookups of the long url for a given short code.
    // It is kept separate from the _urlStore to avoid contention on the click count writes.
    private readonly ConcurrentDictionary<string, string> _urlRedirectMap = new();
    private const string BaseUrl = "http://localhost:5226/";
    private readonly Random _random = new();

    public Task<TinyUrlRecord> CreateTinyUrlAsync(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            throw new ArgumentException("Long URL is required");
        }

        if (!Uri.TryCreate(longUrl, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException($"\"{longUrl}\" is not a valid URL");
        }

        string shortCode;
        do
        {
            shortCode = GenerateShortCode();
        } while (_urlStore.ContainsKey(shortCode));

        var tinyUrl = new TinyUrlRecord(
            Id: shortCode,
            LongUrl: longUrl,
            ShortUrl: $"{BaseUrl}{shortCode}",
            ClickCount: 0
        );

        _urlStore[shortCode] = tinyUrl;
        _urlRedirectMap[shortCode] = longUrl;
        return Task.FromResult(tinyUrl);
    }

    public Task<IEnumerable<TinyUrlRecord>> GetAllUrlsAsync()
    {
        return Task.FromResult(_urlStore.Values.AsEnumerable());
    }

    public Task<string?> GetLongUrlAsync(string id)
    {
        _urlRedirectMap.TryGetValue(id, out var longUrl);
        _ = UpdateClickCountAsync(id); // Fire and forget
        return Task.FromResult(longUrl);
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
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
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
