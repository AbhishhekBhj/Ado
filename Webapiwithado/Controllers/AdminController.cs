using Microsoft.AspNetCore.Mvc;
using Webapiwithado.DataAccess;
using Webapiwithado.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Webapiwithado.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly AdminDataAccess _adminDataAccess;
        private readonly QuizDataAccess quizDataAccess;

        public AdminController(AdminDataAccess adminDataAccess, QuizDataAccess quizDataAccess)
        {
            _adminDataAccess = adminDataAccess;
            this.quizDataAccess = quizDataAccess;
        }


        [HttpPost("AddQuiz")]

        public async Task<IActionResult> AddQuizAsync([FromBody] QuizUpdateDto quizDTO)
        {
            try
            {
                var response = await _adminDataAccess.AddQuizAsync(quizDTO);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/<AdminController>
        [HttpGet("GetAllQuizes")]
        public async Task<IActionResult> GetAllQuizesAsync()
        {
            try
            {
                var response = await _adminDataAccess.GetAllQuizesAsync();

                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<AdminController>/5
        [HttpGet("GetQuizByID/{id}")]
        public async Task<IActionResult> GetQuizByIdAsync([FromRoute] int id)
        {
            try
            {
                var response = await quizDataAccess.GetQuizObjectByIDAsync(id);

                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
        [HttpGet("Get3BestPerformersForQuiz/{id}")]
        public async Task<IActionResult> Get3BestPerformersForQuizAsync([FromRoute]int id)
        {
            try
            {
                var response = await _adminDataAccess.Get3BestPerformersForQuizbyIdAsync(id);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Get3WorstPerformersForQuiz/{id}")]
        public async Task<IActionResult> Get3WorstPerformersForQuizAsync([FromRoute] int id)
        {
            try
            {
                var response = await _adminDataAccess.Get3WorstPerformersForQuizbyIdAsync(id);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<AdminController>/5
        [HttpPut("EditQuiz")]
        public async Task<IActionResult> EditQuizAsync( [FromBody] QuizUpdateDto quizDTO)
        {
            try
            {
                var response = await _adminDataAccess.UpdateQuizAsync( quizDTO);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<AdminController>/5
        [HttpDelete("DeleteQuiz")]
        public async Task<IActionResult> DeleteQuizAsync([FromBody] Dictionary<String, int> deleteMap)
        {
            try
            {
                var response = await _adminDataAccess.DeleteQuizAsync(deleteMap["id"]);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
