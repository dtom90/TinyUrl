namespace TinyUrl.Api.Models;

public record TinyUrlRequest(string LongUrl, string? ShortCode);
public record TinyUrlRecord(string Id, string LongUrl, string ShortUrl, int ClickCount);
public record ErrorResponse(string error);
