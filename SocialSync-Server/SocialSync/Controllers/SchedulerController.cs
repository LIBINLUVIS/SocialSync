using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using SocialSyncBusiness.IServices;
using SocialSyncData.Data;
using SocialSyncData.Models;
using SocialSyncDTO.Constants;
using SocialSyncDTO.DTOs;

namespace SocialSync.Controllers;

[ApiController]
[Route("[controller]")]
public class SchedulerController : ControllerBase
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ISchedulerService _schedulerService;
    private readonly SocialSyncDataContext _dbContext;
    
    public SchedulerController(ISchedulerFactory schedulerFactory,
        ISchedulerService schedulerService,SocialSyncDataContext dbContext)
    {
        _schedulerFactory = schedulerFactory;
        _schedulerService = schedulerService;
        _dbContext = dbContext;
    
    }

    [HttpPost("ScheduleLinkedinTextOnlyPost")]
    public async Task<IActionResult> LinkedinTextOnlyPost([FromQuery] DateTime scheduledDateTime,
        [FromQuery] int UserID,[FromQuery] string text,[FromQuery] string AccessToken,[FromQuery] List<string> PageId,
        [FromQuery] string Timezone,[FromQuery] List<string> pageName)
    {
        
        var scheduledTime = scheduledDateTime;
        Provider provider = Provider.Linkedin;
        TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Timezone);
        DateTime userLocalTime = TimeZoneInfo.ConvertTimeFromUtc(scheduledDateTime, userTimeZone);
        List<int> PostSchedulerIds = new List<int>();
        var resultUrn = pageName[0].Trim('[', ']').Split(',');
        if (scheduledTime <= DateTime.UtcNow)
        {
            return Ok(new { message = "Scheduling must be in future", statusCode = 404, success = false });
        }
         //move this to a service : TODO
        foreach (var pagename in pageName)
        {
            try
            {
                var postSchedulerobj = new PostScheduler()
                {
                    ScheduledDateTime = userLocalTime,
                    Isposted = false,
                    PageName = pagename,
                    PostCommentary = text,
                    ProviderName = provider.ToString(),
                    UseraccountId = UserID
                };
                await _dbContext.PostSchedulers.AddAsync(postSchedulerobj);
                await _dbContext.SaveChangesAsync();
                
                PostSchedulerIds.Add(postSchedulerobj.PostSchedulerId);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
             _schedulerService.LinkedinTextOnlyPost(scheduledDateTime,UserID,text,AccessToken,PageId,
             Timezone,pageName,PostSchedulerIds);

         return Ok(new {message = $"TextonlyPostScheduled at : {scheduledDateTime}",
             StatusCode = 200,
             Success=true });
    }
    
    [HttpPost("ScheduleLinkedinPost")]
    public async Task<IActionResult> LinkedinPost([FromForm] IFormFile file,[FromForm] List<string> PageId,[FromForm] string AccessToken,
        [FromForm]  int userId,[FromForm] string PostText,[FromForm] DateTime scheduledDateTime)
    {
        
        var scheduledTime = scheduledDateTime;

        if (scheduledTime <= DateTime.UtcNow)
        {
            return BadRequest("Scheduled time must be in the future.");
        }
        
        _schedulerService.LinkedinPost(file,PageId,AccessToken,userId,PostText,scheduledTime);

        return Ok(new {message = $"PostScheduled at : {scheduledDateTime}"});
    }

    [HttpGet("MySchedules")]
    public async Task<IActionResult> MySchedules(int useraccountId)
    {
        var req = await _schedulerService.myschedules(useraccountId);
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