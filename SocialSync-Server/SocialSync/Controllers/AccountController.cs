using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialSyncBusiness.IServices;
using SocialSyncData.Models;
using SocialSyncDTO.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;

namespace SocialSync.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {

        //private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;
        private readonly IAccountService _accountService;

        public AccountController(
            
            //UserManager<User> userManager, SignInManager<User> signInManager,
            IAccountService accountService
            )
        {
            //_userManager = userManager;
            //_signInManager = signInManager;
            _accountService = accountService;
        }




        [HttpPost("userRegister")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
        {

            if (ModelState.IsValid)
            {
                var result = await _accountService.SignupUser(model);


                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest("Model is invalid");
            }

            
        }

       
        //[HttpPost("userlogin")]
        //public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        //{
        //    var result = await _accountService.SignInUser(model);
        //    return Ok(result);
        //}

        //[HttpPost("Userlogin")]
        //public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        //{
        //    var result = await _accountService.SignInUser(model);
        //    return Ok(result);
        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.SignInUser(model);
                return Ok(result);
            }
            else
            {
                return BadRequest("Model is invalid");
            }
        }

        [HttpGet("Test")]
        public async Task<IActionResult> Fuck()
        {
            return Ok(new { message = "hello world" });
        }

        [EnableRateLimiting("IPBasedOTPLimiter")]
        [HttpPost("Forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.ForgotPassword(model);
         
                return Ok(result);
 
            }
            else
            {
                return BadRequest(ModelState);
            }

        }
        

        [HttpGet("VerifyCode")]
        public async Task<IActionResult> VerifyCode(string email, string code)
        {
            var result = await _accountService.VerifyCode(email, code);
            return Ok(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string email,string newPassword)
        {
            var result = await _accountService.ResetPassword(email, newPassword);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }





    }
}
