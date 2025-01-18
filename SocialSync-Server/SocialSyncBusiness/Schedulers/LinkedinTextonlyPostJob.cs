using System.Net.Http.Headers;
using System.Text;
using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using SocialSyncData.Data;
using SocialSyncData.Models;
using SocialSyncDTO.Constants;
using SocialSyncDTO.DTOs;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SocialSyncBusiness.Schedulers;

public class LinkedinTextonlyPostJob : IInvocable,IInvocableWithPayload<LinkedinTextOnlyPostSchedulerDto>
{
    public LinkedinTextOnlyPostSchedulerDto Payload { get; set; }
    private readonly ILogger<LinkedinTextonlyPostJob> _logger;
    private readonly IConfiguration _configuration;
    private readonly SocialSyncDataContext _dbcontext;
    
    public LinkedinTextonlyPostJob(ILogger<LinkedinTextonlyPostJob> logger, 
        IConfiguration configuration, SocialSyncDataContext dbcontext)
    {
        _logger = logger;
        _configuration = configuration;
        _dbcontext = dbcontext;
    }
    
    public async Task Invoke()
    {
        _logger.LogInformation("LinkedinTextonlyPostJob Invoked");
        var accessToken = Payload.AccessToken ?? "";
        var postText = Payload.PostText ?? "";
        List<string> PageIds = Payload.PageIds ?? new List<string>();
        var UserId = Payload.UserId;
        await LinkedinTextonlyPostTask(postText,accessToken,PageIds,UserId);
        
    }

    public async Task LinkedinTextonlyPostTask(string Text,string AccessToken,List<string> PageIdList,
        int userID)
    {
        var TextOnlyPostApi = "https://api.linkedin.com/rest/posts";
        Provider provider = Provider.Linkedin;
        List<string> PostId = new List<string>();
        List<int> SchedulerIds = Payload.ScheduleIds;
        Console.WriteLine(SchedulerIds);
        var Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("X-Restli-Protocol-Version","2.0.0");
        Client.DefaultRequestHeaders.Add("LinkedIn-Version","202405");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        try
        {
            foreach (var pageId in PageIdList)
            {
                var payload = new
                {
                    author = pageId,
                    commentary = Text,
                    visibility = "PUBLIC",
                    distribution = new
                    {
                        feedDistribution = "MAIN_FEED",
                        targetEntities = new string[] { },
                        thirdPartyDistributionChannels = new string[] { },
                    },
                    lifecycleState = "PUBLISHED",
                    isReshareDisabledByAuthor = false
                };
                var jsonPayload = JsonSerializer.Serialize(payload);
        
                var req = await Client.PostAsync(TextOnlyPostApi, 
                    new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
                var reqContent = await req.Content.ReadAsStringAsync();
                Console.WriteLine(reqContent);
                if (req.IsSuccessStatusCode)
                {
                    Console.WriteLine(reqContent);
                    if (req.Headers.Contains("x-restli-id"))
                    {
                        var postID = req.Headers.GetValues("x-restli-id").FirstOrDefault();
                        /*PostId = postID;*/
                        PostId.Add(postID);
                        try
                        {
                            foreach (var postScheduler in SchedulerIds)
                            {
                                var userPostObj = new UserPosts()
                                {
                                    PostId = postID,
                                    DirectPost = false,
                                    PostedOn = DateTime.Now,
                                    ProviderName = provider.ToString(),
                                    PostType = "TextOnly",
                                    UseraccountId = userID,
                                    PostSchedulerId = postScheduler,
                                };
                                await _dbcontext.userposts.AddAsync(userPostObj);
                                await _dbcontext.SaveChangesAsync();
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        //update my status of the schedulertable
                        foreach (var id in SchedulerIds)
                        {
                            var postSchedule = await _dbcontext.PostSchedulers.Where(s=>s.PostSchedulerId==id)
                                .FirstOrDefaultAsync();
                            if (postSchedule != null)
                            {
                                postSchedule.Isposted = true;
                                await _dbcontext.SaveChangesAsync();
                            }

                        }
                        
                    }
                }
                else
                {
                    Console.WriteLine($"Error :{reqContent}");
                    /*_logger.LogInformation($"Exception thrown at else case : {DateTime.Now}");*/
                    throw new Exception(req.ReasonPhrase);
                }
            }
            
            /*_logger.LogInformation($"Posted Successfully at : {DateTime.Now}");*/

        }
        catch (Exception e)
        {
            /*_logger.LogInformation($"Exception thrown at catch : {DateTime.Now}");*/
            Console.WriteLine($"Form the catch block, Error : {e.Message}");
            throw new Exception(e.Message);
        }

    }

}