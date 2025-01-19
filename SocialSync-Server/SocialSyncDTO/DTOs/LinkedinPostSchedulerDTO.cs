namespace SocialSyncDTO.DTOs;

public class LinkedinPostSchedulerDTO
{
    /*IFormFile file, List<string> PageId, string AccessToken,
    int userId, string PostText, DateTime scheduledDateTime*/
    public string? filebase64 { get; set; }
    public List<string>? PageIds { get; set; }
    public string? AccessToken { get; set; }
    public int UserId { get; set; }
    public string? PostText { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    
}