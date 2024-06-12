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


        [HttpGet("GetDashBoardData")]

        public async Task<IActionResult> GetDashBoardDataAsync()
        {
            try
            {
                var response = await _adminDataAccess.GetDashBoardDataAsync();
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllUsers/{pageNumber}/{rowsPerPage}")]

        public async Task<IActionResult> GetAllUsersAsync([FromRoute] int pageNumber, int rowsPerPage)
        {
           
            try
            {
                var response = await _adminDataAccess.GetAllUsersAsync(pageNumber,rowsPerPage);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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

        [HttpPost("AddNewQuestion")]

        public async Task<IActionResult> AddNewQuestionAsync([FromBody] AddQuestionDTO questionDTO)
        {
            try
            {
                var response = await _adminDataAccess.AddQuestionAsync(questionDTO);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<AdminController>/5
        [HttpDelete("DeleteQuiz")]
        public async Task<IActionResult> DeleteQuizAsync([FromBody] Dictionary<string, int> deleteMap)
        {
            try
            {
                var response = await _adminDataAccess.DeleteQuizAsync(Convert.ToInt32(deleteMap["id"]));
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("DeleteQuestion/{id}")]

        public async Task<IActionResult> DeleteQuestionAsync([FromRoute] int id)
        {
            try
            {
                var response = await _adminDataAccess.DeleteQuestionAsync(Convert.ToInt32(id));
                return Ok(response);

            }

            catch(Exception ex)
            {

                return(StatusCode(500, ex.Message));


            }
        }

        [HttpPut("UpdateQuestionDetails")]

        public async Task<IActionResult> UpdateQuestionAsync([FromBody] UpdateQuestionDTO updateQuestionDTO)
        {
            try
            {
                var response = await _adminDataAccess.UpdateQuestionAsync(updateQuestionDTO);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }









        [HttpGet("GetAllContentType")]
        public async Task<IActionResult> GetAllContentTypeAsync()
        {
            try
            {
                var response = await _adminDataAccess.GetAllContentTypeAync();
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
