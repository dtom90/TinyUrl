using TinyUrl.Api.Models;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;

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
    // This dictionary stores all the data related to the tiny urls, including the click count.
    // It will experience more frequent writes, due to the need to update the click count.
    private readonly ConcurrentDictionary<string, TinyUrlRecord> _urlStore = new();
    
    // This dictionary is used for quick lookups of the long url for a given short code.
    // It is kept separate from the _urlStore to avoid contention on the click count writes.
    private readonly ConcurrentDictionary<string, string> _urlRedirectMap = new();
    private const string BaseUrl = "http://localhost:5226/";
    private readonly Random _random = new();

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
        if (string.IsNullOrWhiteSpace(request.ShortCode))
        {
            do
            {
                shortCode = GenerateShortCode();
            } while (_urlStore.ContainsKey(shortCode));
        }
        else
        {
            // Check if short code is a valid format
            if (!Regex.IsMatch(request.ShortCode, "^[a-zA-Z0-9]+$"))
            {
                throw new ArgumentException($"Short code \"{request.ShortCode}\" is not valid (only alphanumeric characters are allowed)");
            }
            if (_urlStore.ContainsKey(request.ShortCode))
            {
                throw new ArgumentException($"Short code \"{request.ShortCode}\" is already in use");
            }
            shortCode = request.ShortCode;
        }

        var tinyUrl = new TinyUrlRecord(
            Id: shortCode,
            LongUrl: request.LongUrl,
            ShortUrl: $"{BaseUrl}{shortCode}",
            ClickCount: 0
        );

        if (!_urlStore.TryAdd(shortCode, tinyUrl))
        {
            Console.Error.WriteLine($"Failed to add to _urlStore for short code '{shortCode}' - it may have been added concurrently");
            throw new InvalidOperationException();
        }
        if (!_urlRedirectMap.TryAdd(shortCode, request.LongUrl))
        {
            _urlStore.TryRemove(shortCode, out _); // If we failed to add to the redirect map, we should clean up the urlStore entry
            Console.Error.WriteLine($"Failed to add to _urlRedirectMap for short code '{shortCode}' - it may have been added concurrently");
            throw new InvalidOperationException();
        }
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
