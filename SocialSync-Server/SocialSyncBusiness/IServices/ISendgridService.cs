using SocialSyncDTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialSyncBusiness.IServices
{
     public interface ISendgridService
    {
        Task<ServiceResult<string>> ForgotpasswordCode(string code,string email);
    }
}
