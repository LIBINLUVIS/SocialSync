using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SocialSyncBusiness.IServices;

namespace SocialSync.Controllers;

[ApiController]
[Route("[controller]")]
public class SocialController : ControllerBase
{
    private readonly ISocialService _socialService;
    public SocialController(ISocialService socialService)
    {
        _socialService = socialService;
    }
    
    [HttpGet("LinkedinAccessToken")]
    public async Task<IActionResult> GetLinkedinAccessToken(string authCode,int accountUserId)
    {
        Console.WriteLine(accountUserId);
        Console.WriteLine(accountUserId.GetType);
        var request = await _socialService.LinkedinAccessToken(authCode,accountUserId);
        return Ok(request);
    }


    
}