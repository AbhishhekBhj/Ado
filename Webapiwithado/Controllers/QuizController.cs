using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webapiwithado.DataAccess;
using Webapiwithado.Models;

namespace Webapiwithado.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly QuizDataAccess _quizDataAccess;

        public QuizController(QuizDataAccess quizDataAccess)
        {
            _quizDataAccess = quizDataAccess;
        }


        [HttpGet]
        [Route("GetAllQuiz")]
        [Authorize] //this end point will now require a token to access

        public async Task<IActionResult> GetAllQuizAsync()
        {
            try
            {
                ResponseModel responseModel = await _quizDataAccess.GetAllQuizAsync();
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[HttpGet]
        //[Route("GetQuizById/{quizId}")]

        //public async Task<IActionResult> GetQuizByIdAsync([FromRoute] int quizId)
        //{
        //    try
        //    {
        //        ResponseModel responseModel = await _quizDataAccess.GetQuizContent(quizId);
        //        return Ok(responseModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}



    }
}
