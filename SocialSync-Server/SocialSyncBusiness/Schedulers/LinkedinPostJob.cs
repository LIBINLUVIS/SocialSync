using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Coravel.Invocable;
using Microsoft.AspNetCore.Http;    
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using SocialSyncData.Data;
using SocialSyncData.Models;
using SocialSyncDTO.Constants;
using SocialSyncDTO.DTOs;

namespace SocialSyncBusiness.Schedulers;

public class LinkedinPostJob : IInvocable,IInvocableWithPayload<LinkedinPostSchedulerDTO>
{
    public LinkedinPostSchedulerDTO Payload { get; set; }
    readonly SocialSyncDataContext _dbcontext;
    readonly IHttpClientFactory _httpClientFactory;
    readonly IConfiguration _configuration;
    readonly ILogger<LinkedinPostJob> _logger;
    

    public LinkedinPostJob(SocialSyncDataContext dbcontext,IHttpClientFactory httpClientFactory,
        IConfiguration configuration,ILogger<LinkedinPostJob> logger)
    {
        _dbcontext = dbcontext;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
       
    }


    
    
    public async Task Invoke()
    {

        List<string> PageIds = Payload.PageIds;
        var AccessToken = Payload.AccessToken;
        string PostText = Payload.PostText;
        string postid = "";
        Provider provider = Provider.Linkedin;
        string[] PageIdArray = PageIds.ToArray();
        string[] resultUrn = new string[0];
        resultUrn = PageIdArray[0].Trim('[', ']').Split(',');
        //base64 to file
        string base64file = Payload.filebase64;
        Console.WriteLine(base64file);
        try
        {
            var file = FileHelper(base64file, "image/jpeg");
            Console.WriteLine(file);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        
        /*foreach (var pageid in resultUrn)
        {
            var initializeData = await InitializeImageUpload(pageid, AccessToken);
            var ImageId = initializeData["ImageId"];
            var ImageUploadUrl = initializeData["ImageUploadUrl"];
            IFormFile file = null;
            await UplaodImage(file,ImageUploadUrl, AccessToken); 
            await CreateImagePostAsync(pageid, ImageId, AccessToken,PostText);
        }*/
    }
    
    public static IFormFile FileHelper(string base64String, string fileName)
    {
        // Decode the Base64 string into a byte array
        Console.WriteLine(base64String,fileName);
        byte[] fileBytes = Convert.FromBase64String(base64String);
        var memoryStream = new MemoryStream(fileBytes);
        
        return new FormFile(memoryStream, 0, fileBytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = GetContentType(fileName)
        };

  
    }
    
    private static string GetContentType(string fileName)
    {
        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        if (provider.TryGetContentType(fileName, out string contentType))
        {
            return contentType;
        }
        return "application/octet-stream"; // Default to binary file type
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
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
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
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
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
    private async Task CreateImagePostAsync(string PageId,string ImageId ,string accessToken,string PostText)
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

    }
    
    
    
}