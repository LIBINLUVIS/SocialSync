using System.ComponentModel.DataAnnotations;

namespace SocialSyncData.Models;

public class Useraccount
{
    
    public int id { get; set; }
    public string UserId { get; set; }
    public string AuthToken { get; set; }
    public DateTime TokenExpiry {  get; set; }
    public DateTime CreatedDate { get; set; }
    
    public ICollection<SocialAccount> SocialAccounts { get; set; } 
}