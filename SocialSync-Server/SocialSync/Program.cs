using Coravel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SocialSyncBusiness.IServices;
using SocialSyncBusiness.Services;
using SocialSyncBusiness.Services.Queue;
using SocialSyncData.Data;
using SocialSyncData.Models;
using System.ComponentModel;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
/*builder.Services.AddSignalR();*/
builder.Services.AddQueue();
builder.Services.AddScoped<ForgotPasswordInvocable>();
builder.Services.AddControllers();
builder.Services.AddScoped<IAccountService,AccountService>();
builder.Services.AddScoped<ISendgridService, SendgridService>();
builder.Services.AddScoped<ISocialService, SocialService>();
builder.Services.AddScoped<DataService>();

builder.Services.AddMemoryCache();

builder.Services.AddRateLimiter(policy =>
{
    policy.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    policy.AddFixedWindowLimiter("IPBasedOTPLimiter", options =>
    {
        options.PermitLimit = 1;
        options.Window = TimeSpan.FromSeconds(10);
        /*options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;*/
    });
});

builder.Services.AddHttpClient("linkedinClient", client =>
{
    var linkedinConfigs = builder.Configuration.GetSection("LinkedinConfig");
    var linkedinXrestliProtocol = linkedinConfigs["X-Restli-Protocol-Version"];
    var linkedinVersion = linkedinConfigs["LinkedIn-Version"];
    
    client.DefaultRequestHeaders.Add("X-Restli-Protocol-Version",linkedinXrestliProtocol);
    client.DefaultRequestHeaders.Add("LinkedIn-Version",linkedinVersion);
});

builder.Services.AddHttpClient("linkedinClient1", client =>
{
    var linkedinConfigs = builder.Configuration.GetSection("LinkedinConfig");
    var linkedinVersion = linkedinConfigs["LinkedIn-Version"];
    
    client.DefaultRequestHeaders.Add("LinkedIn-Version",linkedinVersion);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Basic Swagger Configuration
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "File Upload API",
        Version = "v1"
    });

    // Add File Upload Operation Filter
    options.OperationFilter<FileUploadOperationFilter>();
});



builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<SocialSyncDataContext>()
    .AddApiEndpoints();

builder.Services.AddDbContext<SocialSyncDataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors((config => config.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials().WithOrigins("http://localhost:4200");
})));
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Upload API v1"));
    /*c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Upload API v1")*/
}
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store";
    context.Response.Headers["Pragma"] = "no-cache";
    await next();
});
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();
/*app.MapHub<NotificationService>("/notify");*/
app.MapIdentityApi<User>();

app.Run();

//config for file uplaod in swagger
public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        /*var fileParameters = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile));*/
        var formParameters = context.MethodInfo.GetParameters()
            .Where(p => p.GetCustomAttributes(typeof(FromFormAttribute), false).Any());
        if (formParameters.Any())
        {
            operation.Parameters.Clear();
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                ["file"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                },
                                ["PageId"] = new OpenApiSchema
                                {
                                    Type = "array",
                                    Items = new OpenApiSchema { Type = "string" }
                                },
                                ["AccessToken"] =  new OpenApiSchema
                                {
                                    Type = "string",
                                },
                                ["UserID"] =  new OpenApiSchema
                                {
                                    Type = "integer",
                                },
                                ["PostText"] = new OpenApiSchema
                                {
                                    Type = "string",
                                }
                            },
                            Required = new HashSet<string> { "file","PageId","AccessToken","UserID","PostText" },
                            
                        }
                    }
                }
            };
        }
    }
}
