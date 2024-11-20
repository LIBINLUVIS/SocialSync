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
        Task<ServiceResult<string>> SignupUser(UserRegisterDto model);
        Task<ServiceResult<string>> SignInUser(UserLoginDto model);
        Task<ServiceResult<string>> ForgotPassword(ForgotPasswordDto email);
        Task<ServiceResult<string>> VerifyCode(string email, string code);
        Task<ServiceResult<string>> ResetPassword(string email, string newPassword);
    }
}
