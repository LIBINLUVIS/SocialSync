using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialSyncBusiness.IServices;
using SocialSyncData.Models;
using SocialSyncDTO.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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




        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
        {

            if (ModelState.IsValid)
            {
                var result = await _accountService.SignupUser(model);

                if (result.Success)
                {
                    return Ok("User Registered Successfully.");
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }

            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            var result = await _accountService.SignInUser(model);

            return Ok(result);
        }





    }
}
