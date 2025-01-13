namespace SocialSyncData.Models;

public class UserPosts
{
    public int Id { get; set; }
    public string ProviderName { get; set; }
    public string PostId { get; set; }
    public string PostType { get; set; }
    public DateTime PostedOn { get; set; }
    public Boolean DirectPost { get; set; } = false;
    
    public Useraccount Useraccount { get; set; }
    public int UseraccountId { get; set; }
    
    public PostScheduler? PostScheduler { get; set; }
    public int? PostSchedulerId { get; set; }
}