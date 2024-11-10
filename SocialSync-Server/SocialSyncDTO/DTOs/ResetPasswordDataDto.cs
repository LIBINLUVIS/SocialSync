using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialSyncDTO.DTOs
{
    public class ResetPasswordDataDto
    {
        public string ResetCode { get; set; }
        public string Email { get; set; }
    }
}
