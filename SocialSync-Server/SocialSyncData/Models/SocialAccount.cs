using System.Runtime.InteropServices.JavaScript;

namespace SocialSyncData.Models;

public class SocialAccount
{
    public int id { get; set; }
    public string Provider  { get; set; }
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiry { get; set; }
    public bool IsActive { get; set; } = false;
    public DateTime CreatedDate { get; set; }

    public int UserAccountId { get; set; }
    public Useraccount Useraccount { get; set; }
    
}