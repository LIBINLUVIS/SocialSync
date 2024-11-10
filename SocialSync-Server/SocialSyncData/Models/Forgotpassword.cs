using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialSyncData.Models
{
    public class Forgotpassword
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string Code { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool Isverified { get; set; }

    }
}
