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
                Console.WriteLine(responseModel.Data.Count.ToString());
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetUserScoreForParticularQuiz/{userId}/{quizId}")]
        [Authorize]

        public async Task<IActionResult> GetUserScoreForParticularQuizAsync([FromRoute] int userId, [FromRoute] int quizId)
        {
            try
            {
                ResponseModel responseModel = await _quizDataAccess.GetUserQuizScoreForAParticularQuiz(userId, quizId);
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        [Route("GetUserBest3Quizes/{userId}")]
        [Authorize]

        public async Task<IActionResult> GetUserBest3QuizesAsync([FromRoute] int userId)
        {
            try
            {
                ResponseModel responseModel = await _quizDataAccess.GetUserTop3BestQuizAsync(userId);
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetUserWorst3Quizes/{userId}")]
        [Authorize]

        public async Task<IActionResult> GetUserWorst3QuizesAsync([FromRoute] int userId)
        {
            try
            {
                ResponseModel responseModel = await _quizDataAccess.GetUserTop3WorstQuizAsync(userId);
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        [Route("GetQuizObjectByID/{quizId}")]
        [Authorize]
        

        public async Task<IActionResult> GetQuizObjectByIDAsync([FromRoute] int quizId)
        {
            try
            {
                ResponseModel responseModel = await _quizDataAccess.GetQuizObjectByIDAsync(quizId);
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("SubmitQuizScore")]
        [Authorize]

        public async Task<IActionResult> SubmitQuizScoreAsync([FromBody] SubmitQuizScore submitQuizScore)
        {
            try
            {
                ResponseModel responseModel = await _quizDataAccess.PostQuizScoreAsync(submitQuizScore);
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        

        



    }
}
