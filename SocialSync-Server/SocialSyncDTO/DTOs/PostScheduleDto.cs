namespace SocialSyncDTO.DTOs;

public class PostScheduleDto
{
    public string Pagename { get; set; }
    public string commentary { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Provider { get; set; }
    public bool IsPosted { get; set; }
        
}