using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webapiwithado.DataAccess;
using Webapiwithado.Models;
namespace Webapiwithado.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserDataAccess _userDataAccess;

        public UserController(UserDataAccess userDataAccess)
        {
            _userDataAccess = userDataAccess;
        }

        [HttpGet]
        [Route("GetUserData/{userId}")]
        public async Task<IActionResult> GetUserData(int userId)
        {
            try
            {
                var user = await _userDataAccess.GetUserDataAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("UpdateUserProfile")]

        public async Task<IActionResult> UpdateUserProfile([FromBody]User user)
        {
            try
            {
                var response = await _userDataAccess.UpdateUserProfileAsync(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpDelete]
        [Route("DeleteUser/{userId}")]

        public async Task<IActionResult> DeleteUser([FromRoute]int userId)
        {
            try
            {
                var response = await _userDataAccess.DeleteUserDataAsync(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }




    }
}
