using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SocialSyncBusiness.IServices;
using SocialSyncData.Data;
using SocialSyncData.Models;
using SocialSyncDTO.Constants;
using SocialSyncDTO.DTOs;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SocialSyncBusiness.Services;

public class SocialService: ISocialService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SocialSyncDataContext _dbcontext;
    public SocialService(IHttpContextAccessor httpContextAccessor , SocialSyncDataContext dbcontext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbcontext = dbcontext;
    }
    
    public class  Token
    {
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }
        public int? expires_in { get; set; }


    }

    public async Task<ServiceResult<string>> LinkedinAccessToken(string code,int accountUserId)
    {


        var client = new HttpClient();
        var context = _httpContextAccessor.HttpContext;
        var accessToken = "";

        var url = "https://www.linkedin.com/oauth/v2/accessToken";
        
        var Reqbody = new Dictionary<string, string>
        {
            {"grant_type","authorization_code"},
            {"code", code},
            {"redirect_uri",""},
            {"client_id", ""},
            {"client_secret", ""},
        };
        
        var urlencoded = new FormUrlEncodedContent(Reqbody);

        try
        {
            var res = await client.PostAsync(url, urlencoded);

            if (res.IsSuccessStatusCode)
            {
                var responseData = await res.Content.ReadAsStringAsync();

                var tokenData = JsonSerializer.Deserialize<Token>(responseData);
                Provider provider = Provider.Linkedin;
                accessToken = tokenData.access_token;
               //chekcing the user is already connected to the social account 
               var usersocialaccount = await _dbcontext.socialaccount.Where(x => x.id == accountUserId).FirstOrDefaultAsync();
               if (usersocialaccount != null)
               {
                   usersocialaccount.AccessToken = accessToken;
                   usersocialaccount.AccessTokenExpiry = DateTime.UtcNow.AddDays(1);
                   usersocialaccount.IsActive = true;
                   
                   await _dbcontext.SaveChangesAsync();
               }
               else
               {
                   var socialaccount = new SocialAccount()
                   {
                       AccessToken = accessToken,
                       AccessTokenExpiry = DateTime.Now.AddDays(1),
                       CreatedDate = DateTime.Now,
                       Provider = provider.ToString(),
                       IsActive = true,
                       UserAccountId = accountUserId
                   };
                   
                   await _dbcontext.socialaccount.AddAsync(socialaccount);
                   await _dbcontext.SaveChangesAsync();
               }

            }
            else
            {
                return new ServiceResult<string>()
                {
                    Data = null,
                    Success = false,
                    Message = "AccessToken not granted",
                    StatusCode = 402
                };
            }

            return new ServiceResult<string>()
            {
                Data = accessToken,
                Success = true,
                Message = "Access Token granted Successfully.",
                StatusCode = 200
            };

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;

        }
    }
}