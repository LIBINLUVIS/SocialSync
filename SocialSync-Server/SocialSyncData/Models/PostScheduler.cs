using System.ComponentModel.DataAnnotations;

namespace SocialSyncData.Models;

public class PostScheduler
{
    public int PostSchedulerId { get; set; }
    [MaxLength(100)]
    public string? PageName { get; set; }
    [MaxLength(100)]
    public string? PostCommentary { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public Boolean Isposted { get; set; } = false;
    public string? PostId { get; set; }
    [MaxLength(50)]
    public string? ProviderName { get; set; }
    
    public UserPosts? UserPost { get; set; }
    

}