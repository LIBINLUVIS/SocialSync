using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SocialSyncBusiness.IServices;
using SocialSyncBusiness.Schedulers;
using SocialSyncData.Data;
using SocialSyncData.Models;
using SocialSyncDTO.Constants;
using SocialSyncDTO.DTOs;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

namespace SocialSyncBusiness.Services;

public class SchedulerService : ISchedulerService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IQueue _queue;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly SocialSyncDataContext _dbcontext;
    private readonly IServiceProvider _serviceProvider;
    /*private readonly LinkedinTextonlyPostJob _linkeidntextonlypostTask;*/
    
    public SchedulerService(ISchedulerFactory schedulerFactory,IQueue queue,
        IHttpClientFactory httpClientFactory,IConfiguration configuration,SocialSyncDataContext dbcontext,
        IServiceProvider serviceProvider
        /*LinkedinTextonlyPostJob linkeidntextonlypostTask*/
        )
    {
        _schedulerFactory = schedulerFactory;
        _queue = queue;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _dbcontext = dbcontext;
        _serviceProvider = serviceProvider;
        /*_linkeidntextonlypostTask = linkeidntextonlypostTask;*/
    }
    
    public async Task LinkedinTextOnlyPost(DateTime scheduledDateTime,
        int UserID,string text, string AccessToken,List<string> PageId,string Timezone,
        List<string>PageNames,List<int>PostSchedulerIds)
    {
        Provider provider = Provider.Linkedin;
        TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Timezone);
        DateTime userLocalTime = TimeZoneInfo.ConvertTimeFromUtc(scheduledDateTime, userTimeZone);
        

        LinkedinTextOnlyPostSchedulerDto payload = new LinkedinTextOnlyPostSchedulerDto()
        {
            AccessToken = AccessToken,
            PostText = text,
            PageIds = PageId,
            UserId = UserID,
            ScheduleIds = PostSchedulerIds,
            
        };
        var delay = scheduledDateTime - DateTime.UtcNow;
        await Task.Run(async() =>
        {
            await Task.Delay(delay);
            _queue.QueueInvocableWithPayload<LinkedinTextonlyPostJob, LinkedinTextOnlyPostSchedulerDto>(payload);
        });


    }

    public async Task LinkedinPost(IFormFile file, List<string> PageId, string AccessToken,
        int userId, string PostText, DateTime scheduledDateTime)
    {
        LinkedinPostSchedulerDTO paylaod = new LinkedinPostSchedulerDTO()
        {
           AccessToken = AccessToken,
           PostText = PostText,
           PageIds = PageId,
           UserId = userId,
           filebase64 = await ConvertFileToBase64(file)
         
        };
        
        var delay = scheduledDateTime - DateTime.UtcNow;
        Task.Run(async() =>
        {
            await Task.Delay(delay);
            _queue.QueueInvocableWithPayload<LinkedinPostJob, LinkedinPostSchedulerDTO>(paylaod);
        });

    }
    
    private async Task<string> ConvertFileToBase64(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return Convert.ToBase64String(memoryStream.ToArray());
    }
    
    
   
    
    
}