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
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddQueue();
builder.Services.AddScoped<ForgotPasswordInvocable>();
builder.Services.AddControllers();
builder.Services.AddScoped<IAccountService,AccountService>();
builder.Services.AddScoped<ISendgridService, SendgridService>();

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

app.MapIdentityApi<User>();

app.Run();
