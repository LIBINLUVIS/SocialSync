using System.Net;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace SocialSyncDTO.DTOs;

public class LinkedinTextOnlyPostSchedulerDto 
{
    public string? PostText { get; set; }
    public int UserId { get; set; }
    public List<string> PageIds { get; set; } = new List<string>();
    public string? AccessToken { get; set; }
    public string? filebase64 { get; set; }
    public List<int>? ScheduleIds { get; set; }
}