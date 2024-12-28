using Microsoft.AspNetCore.Http;
using SocialSyncBusiness.Services;
using SocialSyncDTO.DTOs;

namespace SocialSyncBusiness.IServices;

public interface ISocialService
{

    Task<ServiceResult<string>> LinkedinAccessToken(string code,int accountUserId);
    Task<ServiceResult<IEnumerable<LinkedinAdminPages>>> GetLinkedinAdminPages(string accessToken);
    Task<ServiceResult<List<string>>> TextOnlyPost(int UserID,string text,string AccessToken,List<string> PageId);
    Task<ServiceResult<string>> PostImage(IFormFile file,List<string> PageId,string AccessToken,
        int userId,string PostText);

}