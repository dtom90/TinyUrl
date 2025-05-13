using TinyUrl.Api.Models;

namespace TinyUrl.Api.Services;

public interface ITinyUrlService
{
    Task<TinyUrlRecord> CreateTinyUrlAsync(string longUrl);
    Task<IEnumerable<TinyUrlRecord>> GetAllUrlsAsync();
    Task<TinyUrlRecord?> DeleteUrlAsync(string id);
    Task<TinyUrlRecord?> GetUrlAsync(string id);
}
