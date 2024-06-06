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



        [HttpPost("send-verification-email")]
        public async Task<IActionResult> SendVerificationEmail([FromBody] Dictionary<string, string> email)
        {
            try
            {
                string receiverEmail = email["email"];

                // Generate a random OTP
                Random random = new Random();
                int otp = random.Next(100000, 999999);

                // Construct the email body
                string subject = "🎉 Verify Your Email Address - Quiz App 🎉";
                string body = $"👋 Hello Quiz Master!\n\n" +
                              "Thank you for signing up for our quiz app! To complete your registration, please verify your email address by using the OTP (One-Time Password) provided below:\n\n" +
                              $"Your OTP: {otp} 🔐\n\n" +
                              "This OTP is valid for the next 10 minutes. Please do not share this OTP with anyone.\n\n" +
                              "If you did not request this email, please ignore it.\n\n" +
                              "Ready to start quizzing? Let's go! 🚀\n\n" +
                              "Best regards,\nHamro  Quiz App Team 🌟";

                // Send the email
                await _emailSender.SendEmailAsync(receiverEmail, subject, body);
                await _userDataAccess.SetOtpInUserTableAsync(otp, receiverEmail);
                return Ok(new { Message = "Verification email sent successfully", OTP = otp });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while sending the email", Error = ex.Message });
            }
        }


        [HttpPost("verify-otp")]
        //[Authorize]
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
