using SocialSyncDTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialSyncBusiness.IServices
{
    public interface IAccountService
    {
        Task<ServiceResult<bool>> SignupUser(UserRegisterDto model);
        Task<string> SignInUser(UserLoginDto model);
    }
}
