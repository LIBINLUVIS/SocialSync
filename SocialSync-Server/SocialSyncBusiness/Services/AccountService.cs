using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SocialSyncBusiness.IServices;
using SocialSyncData.Models;
using SocialSyncDTO.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SocialSyncBusiness.Services
{
    public class AccountService : IAccountService
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }



        public async Task<ServiceResult<bool>> SignupUser(UserRegisterDto model)
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

                return new ServiceResult<bool> { Success = true };
            }
            else
            {
                return new ServiceResult<bool> { Success = false };

            }
        }

        public async Task<string> SignInUser(UserLoginDto model)
        {
         
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var token = GenerateJwtToken(user);
                return token;
                    }

            else
            {
                var errormsg = "Wrong Credentials";

                return errormsg;
            }


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
