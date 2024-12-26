using Microsoft.Extensions.Caching.Memory;
using SocialSyncBusiness.IServices;
using SocialSyncDTO.DTOs;

namespace SocialSyncBusiness.Services;

public class DataService
{
    private readonly IMemoryCache _cache;
    private readonly ISocialService _socialService;

    public DataService(IMemoryCache cache,ISocialService socialService)
    {
        _cache = cache;
        _socialService = socialService;
    }
    // linkedin admin pages dataservice
    public async Task<ServiceResult<IEnumerable<LinkedinAdminPages>>> GetLinkedinAdminPagesFormCache(string key,string AccessToken)
    {
        if (_cache.TryGetValue(key, out ServiceResult<IEnumerable<LinkedinAdminPages>> pages))
        {
            return pages;
        }
        
        var data = await _socialService.GetLinkedinAdminPages(AccessToken);
        
        var cacheoptions = new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
            SlidingExpiration = TimeSpan.FromSeconds(5) // extending the expiration,if we access within 5 sec.
        };
        _cache.Set(key, data, cacheoptions);
        return data;
    }
}