namespace TinyUrl.Api.Models;

public record TinyUrlRequest(string LongUrl);
public record TinyUrlRecord(string Id, string LongUrl, string ShortUrl);
