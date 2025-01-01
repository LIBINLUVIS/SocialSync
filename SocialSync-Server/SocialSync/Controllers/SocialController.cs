using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SocialSyncBusiness.IServices;
using SocialSyncBusiness.Services;
using SocialSyncDTO.DTOs;

namespace SocialSync.Controllers;

[ApiController]
[Route("[controller]")]
public class SocialController : ControllerBase
{
    private readonly ISocialService _socialService;
    private readonly DataService _dataService;
    public SocialController(ISocialService socialService,DataService dataService)
    {
        _socialService = socialService;
        _dataService = dataService;
    }
    
    [HttpGet("LinkedinAccessToken")]
    public async Task<IActionResult> GetLinkedinAccessToken(string authCode,int accountUserId)
    {
        var request = await _socialService.LinkedinAccessToken(authCode,accountUserId);
        return Ok(request);
    }

    [HttpGet("getLinkedinAdminPages")]
    public async Task<IActionResult> GetLinkedinAdminPages(string accessToken)
    {
        if (accessToken != null)
        {
        //    var data = await _socialService.GetLinkedinAdminPages(accessToken);
        //calling from the cache
        var data = await _dataService.GetLinkedinAdminPagesFormCache("AdminPages_2024_2412",accessToken);
            
            if (data.StatusCode == 400)
            {
                return BadRequest(data);
            }

            if (data.StatusCode == 500)
            {
                return StatusCode(500, data);
            }
            
            return Ok(data);
            
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost("TextOnlyPost")]
    public async Task<IActionResult> TextOnlyPost(int UserID,string text, string AccessToken,List<string> PageId)
    { 
        var data = await _socialService.TextOnlyPost(UserID, text, AccessToken,PageId);
        if (data.Success)
        {
            return Ok(data);
        }
        else
        {
            return BadRequest(data);
        }
    }

    [HttpPost("PostImage")]
    public async Task<IActionResult> PostImage([FromForm] IFormFile file,[FromForm] List<string> PageId,
        [FromForm] string AccessToken,[FromForm] int UserID,[FromForm] string PostText)
    {

        var req = await _socialService.PostImage(file,PageId,AccessToken,UserID,PostText);
        if (req.Success)
        {
            return Ok(req);
        }
        else
        {
            return BadRequest(req);
        }
    }

    [HttpGet("GetlatestpagePosts")]
    public async Task<IActionResult> GetLatestPagePosts([FromBody] PagePostDto model)
    {
        var req = await _socialService.GetpagePosts(model);
        if (req.Success)
        {
            return Ok(req);
        }
        else
        {
            return BadRequest(req);
        }
    }

    [HttpGet("GetPageStatistics")]
    public async Task<IActionResult> GetPageStatistics(string accessToken, string PageUrn)
    {
        var req = await _socialService.GetPageStatistics(accessToken,PageUrn);
        if (req.Success)
        {
            return Ok(req);
        }
        else
        {
            return BadRequest(req);
        }
    }
    
}