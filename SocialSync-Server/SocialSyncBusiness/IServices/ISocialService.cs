using SocialSyncBusiness.Services;
using SocialSyncDTO.DTOs;

namespace SocialSyncBusiness.IServices;

public interface ISocialService
{

    Task<ServiceResult<string>> LinkedinAccessToken(string code,int accountUserId);

}