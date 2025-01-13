using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Http;

namespace SocialSyncBusiness.IServices;

public interface ISchedulerService 
{
    Task LinkedinTextOnlyPost(DateTime scheduledDateTime,
        int UserID,string text, string AccessToken,List<string> PageId,string Timezone,
        List<string>PageNames,List<int>PostSchedulerIds);

    Task LinkedinPost(IFormFile file, List<string> PageId, string AccessToken,
        int userId, string PostText, DateTime scheduledDateTime);
}