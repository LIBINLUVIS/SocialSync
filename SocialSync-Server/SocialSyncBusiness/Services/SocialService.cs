using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    public SocialService(IHttpContextAccessor httpContextAccessor , SocialSyncDataContext dbcontext,
        IHttpClientFactory httpClientFactory,IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbcontext = dbcontext;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        
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
            {"redirect_uri","http://localhost:4200/accounts"},
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

    public async Task<ServiceResult<IEnumerable<LinkedinAdminPages>>> GetLinkedinAdminPages(string AccessToken)
    {
        try
        {
            var linkedinSec = _configuration.GetSection("LinkedinApis");
            var getAdminPageApi = linkedinSec["GetAdminPages"];
            var linkedinClient = _httpClientFactory.CreateClient("linkedinClient");
            linkedinClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var response = await linkedinClient.GetAsync(getAdminPageApi);
            var responseContent = await response.Content.ReadAsStringAsync();
            List<LinkedinAdminPages> linkedinAdminPages = new List<LinkedinAdminPages>();
            if (response.IsSuccessStatusCode)
            {
                using var jsonDoc = JsonDocument.Parse(responseContent);
                JsonElement root = jsonDoc.RootElement;
                
                if (root.TryGetProperty("elements", out JsonElement elements))
                {
                    foreach (JsonElement element in elements.EnumerateArray())
                    {
                        if (element.TryGetProperty("organization", out JsonElement organization))
                        {
                            string orgurn = organization.GetString();
                            if (!string.IsNullOrEmpty(orgurn))
                            {
                                string urn = orgurn;
                                string[] parts = urn.Split(':');
                                string urnId = parts[^1];

                                string OrgName = await getAdminPagebyId(urnId,AccessToken);
                                int FollowersCount = await getAdminPageFollowersCount(urn,AccessToken);

                                var adminpageObj = new LinkedinAdminPages()
                                {
                                    OrgName = OrgName,
                                    OrgId = urn,
                                    FollowersCount = FollowersCount,
                                };
                                
                                linkedinAdminPages.Add(adminpageObj);
                            }
                        }
                    }
                }
            }
            else
            {
                return new ServiceResult<IEnumerable<LinkedinAdminPages>>()
                {
                    Data = null,
                    Success = false,
                    StatusCode = 400,
                    ErrorMessage = $"Error at getting admin pages {responseContent}",
                };
            }

            return new ServiceResult<IEnumerable<LinkedinAdminPages>>()
            {
                Data = linkedinAdminPages,
                Success = true,
                StatusCode = 200,
                Message = "Success!"
            };

        }
        catch (Exception e)
        {

            return new ServiceResult<IEnumerable<LinkedinAdminPages>>()
            {
                Data = null,
                Success = false,
                ErrorMessage = $"Failed to fetch the admin pages: {e.Message}",
                StatusCode = 500
            };
        }
        
    }

    public async Task<ServiceResult<List<string>>> TextOnlyPost(int UserID,string text,string AccessToken,List<string> PageId)
    {
        var LinkedinApiSec = _configuration.GetSection("LinkedinApis");
        var TextOnlyPostApi = LinkedinApiSec["TextOnlyPost"];
        Provider provider = Provider.Linkedin;
        List<string> PostId = new List<string>();
        var Client = _httpClientFactory.CreateClient("linkedinClient");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        try
        {
            foreach (var pageId in PageId)
            {
                var payload = new
                {
                    author = pageId,
                    commentary = text,
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
                if (req.IsSuccessStatusCode)
                {
                    if (req.Headers.Contains("x-restli-id"))
                    {
                        var postID = req.Headers.GetValues("x-restli-id").FirstOrDefault();
                        /*PostId = postID;*/
                        PostId.Add(postID);
                        var userPostObj = new UserPosts()
                        {
                            PostId = postID,
                            DirectPost = true,
                            PostedOn = DateTime.Now,
                            ProviderName = provider.ToString(),
                            PostType = "TextOnly",
                            UseraccountId = UserID
                        };
                        await _dbcontext.userposts.AddAsync(userPostObj);
                        await _dbcontext.SaveChangesAsync();
                    }
                }
                else
                {
                    return new ServiceResult<List<string>>()
                    {
                        Data = null,
                        Success = false,
                        StatusCode = 400,
                        ErrorMessage = $"Error during Post: {reqContent}",
                    };
                }
            }

        }
        catch (Exception e)
        {
            return new ServiceResult<List<string>>()
            {
                Data = null,
                Success = false,
                StatusCode = 500,
                ErrorMessage = $"Internal Server Error: {e.Message}"
            };
        }

        return new ServiceResult<List<string>>()
        {
            Data = PageId,
            Success = true,
            StatusCode = 200,
            Message = "Success!"
        };

    }

    public async Task<ServiceResult<string>> PostImage(IFormFile file, List<string> PageId, string AccessToken,
        int userId,string PostText)
    {
        try
        {
            string postid = "";
            Provider provider = Provider.Linkedin;
            string[] PageIdArray = PageId.ToArray();
            string[] resultUrn = new string[0];
            resultUrn = PageIdArray[0].Trim('[', ']').Split(',');
            foreach  (var pageid in resultUrn)
            {
                
                //initialize the image upload
                var initializeData = await InitializeImageUpload(pageid, AccessToken);
                var ImageId = initializeData["ImageId"];
                var ImageUploadUrl = initializeData["ImageUploadUrl"];
                await UplaodImage(file,ImageUploadUrl, AccessToken);
                postid = await CreateImagePostAsync(pageid, ImageId, AccessToken,PostText);
                var userPostObj = new UserPosts()
                {
                    PostId = postid,
                    DirectPost = true,
                    PostedOn = DateTime.Now,
                    ProviderName = provider.ToString(),
                    PostType = "ImagePost",
                    UseraccountId = userId
                };
                await _dbcontext.userposts.AddAsync(userPostObj);
                await _dbcontext.SaveChangesAsync();

            }

            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return new ServiceResult<string>()
        {
            Data = null,
            Success = true,
            Message = "Success!",
            StatusCode = 201
        };

    }


    private async Task<string> CreateImagePostAsync(string PageId,string ImageId ,string accessToken,string PostText)
    {
        string postId = "";
        var linkedinApisec = _configuration.GetSection("LinkedinApis");
        var postApi = linkedinApisec["Post"];
        var Client = _httpClientFactory.CreateClient("linkedinClient");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var paylaod = new
        {
            author = PageId,
            commentary = PostText,
            visibility = "PUBLIC",
            distribution = new
            {
                feedDistribution = "MAIN_FEED",
                targetEntities = new string[] { },
                thirdPartyDistributionChannels = new string[] { }
            },
            content = new
            {
                media = new
                {
                    title = "Uploaded Image",
                    id = ImageId
                }
            },
            lifecycleState = "PUBLISHED",
            isReshareDisabledByAuthor = false
        };
        var jsonPayload = JsonSerializer.Serialize(paylaod);
        var req = await Client.PostAsync(postApi,new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
        var reqContent = await req.Content.ReadAsStringAsync();
        if (req.IsSuccessStatusCode)
        {
            if (req.Headers.Contains("x-restli-id"))
            {
                var postID = req.Headers.GetValues("x-restli-id").FirstOrDefault();
                postId = postID;
            }
        }

        return postId;

    }

    private async Task UplaodImage(IFormFile file,string ImageUplaodUrl, string accessToken)
    {
        Console.WriteLine(ImageUplaodUrl);
        Console.WriteLine(file);
        Console.WriteLine(accessToken);
        var client = new HttpClient();
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is missing or empty.");
        using var content = new StreamContent(file.OpenReadStream());
        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
        using var request = new HttpRequestMessage(HttpMethod.Put, ImageUplaodUrl) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private async Task<Dictionary<string, string>> InitializeImageUpload(string pageId, string accessToken)
    {
        try
        {
            Console.WriteLine(pageId);
            var Client = _httpClientFactory.CreateClient("linkedinClient");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var LinekdinapiSec = _configuration.GetSection("LinkedinApis");
            var InitializeImageUploadAPi = LinekdinapiSec["InitializeImageUpload"];
            var payload = new
            {
                
                initializeUploadRequest = new  
                {
                owner = pageId
                }
                
            };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var req = await Client.PostAsync(InitializeImageUploadAPi,new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
            var reqContent = await req.Content.ReadAsStringAsync();
            string imageId = "";
            string imageUplaodUrl = "";
            Console.WriteLine(reqContent);
            if (req.IsSuccessStatusCode)
            {
                Console.WriteLine(reqContent);
                using var jsonDoc = JsonDocument.Parse(reqContent);
                JsonElement root = jsonDoc.RootElement;
                if (root.TryGetProperty("value", out JsonElement value))
                {
                    if (value.TryGetProperty("image", out JsonElement image))
                    {
                        imageId = image.GetString();
                    }

                    if (value.TryGetProperty("uploadUrl", out JsonElement uploadUrl))
                    {
                        imageUplaodUrl = uploadUrl.GetString();
                    }
                }

            }

            return new Dictionary<string, string>()
            {
                {"ImageId" , imageId},
                {"ImageUploadUrl" , imageUplaodUrl},
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<string> getAdminPagebyId(string orgId,string AccessToken)
    {
        var linkedinApiSec = _configuration.GetSection("LinkedinApis");
        var adminpagedetailApi = linkedinApiSec["GetOrgPageDetails"];
        
        var linkedinClient = _httpClientFactory.CreateClient("linkedinClient");
        linkedinClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        string orgName = "";
        var req = await linkedinClient.GetAsync($"{adminpagedetailApi}{orgId}");
        var reqContent = await req.Content.ReadAsStringAsync();
        if (req.IsSuccessStatusCode)
        {
            
            JsonDocument JsDoc = JsonDocument.Parse(reqContent);
            JsonElement root = JsDoc.RootElement;

            if (root.TryGetProperty("localizedName", out JsonElement localizedName))
            {
                orgName = localizedName.GetString();
            }
        }
        
        return orgName;
    }

    private async Task<int> getAdminPageFollowersCount(string orgId, string AccessToken)
    {
        int followersCount = 0;
        var linkedinApiSec = _configuration.GetSection("LinkedinApis");
        var FollowersCountApi = linkedinApiSec["GetPageFollowers"];
        var Client = _httpClientFactory.CreateClient("linkedinClient1");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        var req = await Client.GetAsync($"{FollowersCountApi}{orgId}?edgeType=COMPANY_FOLLOWED_BY_MEMBER");
        var reqContent = await req.Content.ReadAsStringAsync();
        if (req.IsSuccessStatusCode)
        {
            JsonDocument JsDoc = JsonDocument.Parse(reqContent);
            JsonElement root = JsDoc.RootElement;
            if (root.TryGetProperty("firstDegreeSize", out JsonElement firstDegreeSize))
            {
                followersCount = firstDegreeSize.GetInt32();
            }
        }
        
        return followersCount;
    }
}