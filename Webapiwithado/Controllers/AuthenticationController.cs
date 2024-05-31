using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Webapiwithado.DataAccess;
using Webapiwithado.Models;

namespace Webapiwithado.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly UserDataAccess _userDataAccess;


        public AuthenticationController(IConfiguration configuration, UserDataAccess userDataAccess)
        {
            _configuration = configuration;
            _userDataAccess = userDataAccess;

        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] User user)
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
