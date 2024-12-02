using Azure.Core;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;
using SocialSyncBusiness.IServices;
using SocialSyncBusiness.Services.Queue;
using SocialSyncData.Data;
using SocialSyncData.Models;
using SocialSyncDTO.DTOs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SocialSyncBusiness.Services
{
    public class AccountService : IAccountService
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQueue _queue;
        private readonly SocialSyncDataContext _dbContext;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, 
            IHttpContextAccessor httpContextAccessor,IQueue queue, SocialSyncDataContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _queue = queue;
            _dbContext = dbContext;
        }



        public async Task<ServiceResult<string>> SignupUser(UserRegisterDto model)
        {
            var user = new User
            {

                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

          

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                return new ServiceResult<string> { Success = true,
                    Message = "User created successfully.",
                    StatusCode = 201
                };
            }
            else
            {
                return new ServiceResult<string> { Success = false,
                    Message = "Oops User not registered",
                    StatusCode = 400
                };

            }
        }

        public async Task<ServiceResult<string>> SignInUser(UserLoginDto model)
        {
         
          var user = await _userManager.FindByEmailAsync(model.email);
          
          if (user != null && await _userManager.CheckPasswordAsync(user, model.password))
          {
          var token = GenerateJwtToken(user);
          if (token != null)
          {
              var Isuseraccount = await _dbContext.useraccount.Where(u => u.UserId == user.Id).FirstOrDefaultAsync();
              if (Isuseraccount != null)
              {
                  Isuseraccount.AuthToken = token;
                  Isuseraccount.TokenExpiry = DateTime.Now.AddHours(1);
                  await _dbContext.SaveChangesAsync();

              }
              else
              {
              var useraccountObj = new Useraccount()
              {
                  AuthToken = token,
                  CreatedDate = DateTime.Now,
                  TokenExpiry = DateTime.Now.AddHours(1),
                  UserId = user.Id
              };
              await _dbContext.useraccount.AddAsync(useraccountObj);
              await _dbContext.SaveChangesAsync();
          }
          }
          return new ServiceResult<string>
          {
              Success = true,
              Message = "User Login Successfully.",
              StatusCode = 200,
              Data = token  
          };

          }

            else
            {
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = "User Login failed",
                    StatusCode = 402
                };
            }


        }

        public async Task<ServiceResult<string>> ForgotPassword(ForgotPasswordDto email)
        {
            var user = await _userManager.FindByEmailAsync(email.Email);
            if (user == null)
            {
                return new ServiceResult<string>
                {
                    Data = null,
                    Success = false,
                    ErrorMessage = "User was not found",
                    StatusCode = 404
                };

            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            //var resetLink = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/reset-password?token={resetToken}&email={email.Email}";
            // generating a random code for password reset
            string Code =  GeneraterandomCode();
            //check the user is there or not and if not create a new record 
            var user_record = await _dbContext.forgotpassword.Where(u => u.UserEmail == email.Email).SingleOrDefaultAsync();
            if (user_record == null)
            {
                var user_record_obj = new Forgotpassword()
                {
                    UserEmail = email.Email,
                    Code = Code,
                    CreationTime = DateTime.UtcNow,
                    ExpirationTime = DateTime.UtcNow.AddHours(1),
                    Isverified = false
                };

                await _dbContext.forgotpassword.AddAsync(user_record_obj);
                await _dbContext.SaveChangesAsync();
            }
            // else update the fields accordingly
            else
            {
                user_record.Code = Code;
                user_record.CreationTime = DateTime.UtcNow;
                user_record.ExpirationTime = DateTime.UtcNow.AddHours(1);
                user_record.Isverified = false;
                
                await _dbContext.SaveChangesAsync();
            }

         
            
            ResetPasswordDataDto restpassworddto = new ResetPasswordDataDto
            {
                ResetCode = Code,
                Email = email.Email
            };
            //code is added to the queue then it sends through the mail
            _queue.QueueInvocableWithPayload<ForgotPasswordInvocable, ResetPasswordDataDto>(restpassworddto);
            return new ServiceResult<string>
            {
                Success = true,
                Message = "Please check your Email!",
                StatusCode = 200
            };

        }

        public async Task<ServiceResult<string>> VerifyCode(string email, string code)
        {
            //check the user is there or not 
            var user_record = await _dbContext.forgotpassword.Where(u => u.UserEmail == email && u.Code == code).SingleOrDefaultAsync();
            if (user_record == null)
            {
                return new ServiceResult<string>
                {
                    StatusCode = 404,
                    Message = "User not found",
                    Success = false
                };
            }
            else
            {
                // check the code expiry time
                bool Isexpired = user_record.ExpirationTime >= DateTime.UtcNow;
                if (!Isexpired)
                {
                    return new ServiceResult<string>
                    {
                        StatusCode = 401,
                        Message = "Code is expired",
                        Success = false
                    };
                }
                else
                {
                    user_record.Isverified = true;

                    await _dbContext.SaveChangesAsync();

                    return new ServiceResult<string>
                    {
                        StatusCode = 200,
                        Message = "Code is Verified",
                        Success = true
                    };

                    
                }
            
            }
                

        }

        public async Task<ServiceResult<string>> ResetPassword(string email,string newPassword)
        {
            var userobj = await _dbContext.forgotpassword.Where(u => u.UserEmail == email && u.Isverified == true).FirstOrDefaultAsync();
            if(userobj != null)
            {

                // logic for password reset in identity
                var user = await _userManager.FindByEmailAsync(email);

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
               
                var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

                if (result.Succeeded)
                {
                    return new ServiceResult<string>
                    {
                        StatusCode = 200,
                        Success = true,
                        Message = "Password has reseted Successfully"
                    };
                }
                else
                {
                    return new ServiceResult<string>
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = "please check your new password, password reset went wrong"
                    };
                }

            }
            else 
            {

                return new ServiceResult<string>
                {
                    StatusCode = 404,
                    Success = false,
                    Message = "Invalid User or Not verified the Email"
                };

            }

        }


        private static string GeneraterandomCode()
        {
            Random random = new Random();
            return string.Join("",Enumerable.Range(0,6).Select(_=> random.Next(0,10)));

        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("A9G+7Sd1Zp1f2S4+yKcD/Tv0+QX/G1FOMhg0c6pFWTc="); // Replace with your secure key
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
            }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiry
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "https://libinluvis.netlify.app", // Replace with your Issuer
                Audience = "SocialSync-Clients"    // Replace with your Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }




    }
}
