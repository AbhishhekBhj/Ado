using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Webapiwithado.DataAccess;
using Webapiwithado.Models;
using Webapiwithado.ExternalFunctions;
using Webapiwithado.Interface; // Ensure this is added to use your EmailSender

namespace Webapiwithado.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserDataAccess _userDataAccess;
        private readonly IEmailSender _emailSender;

        public AuthenticationController(IConfiguration configuration, UserDataAccess userDataAccess, IEmailSender emailSender)
        {
            _configuration = configuration;
            _userDataAccess = userDataAccess;
            _emailSender = emailSender;
        }

       


        [HttpPost("verify-otp")]
        [Authorize]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerfifyModel otpVerfifyModel)
        {
            try
            {
                var response = await _userDataAccess.CheckandVerifyOTPAsync(otpVerfifyModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel user)
        {
            try
            {
                var response = await _userDataAccess.CreateNewUser(user);
              
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {
            try
            {
                var response = await _userDataAccess.LoginUser(user);

                if (response.Status == 200)
                {
                    return Ok(response);
                }
                else if (response.Status == 401)
                {
                    return Unauthorized(response);
                }
                else if (response.Status == 404)
                {
                    return NotFound(response);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
