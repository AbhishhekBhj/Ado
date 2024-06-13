using Microsoft.AspNetCore.Mvc;
using Webapiwithado.DataAccess;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Webapiwithado.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LessonController : ControllerBase
    {


        private readonly LessonDataAccess _lessonDataAccess;

        public LessonController(LessonDataAccess lessonDataAccess)
        {
            _lessonDataAccess = lessonDataAccess;
        }


        // GET: api/<LessonController>
        [HttpGet]
        [Route("GetContent/{contentid}")]
       public  async Task<IActionResult> GetAllContentByIDAsync([FromRoute] int contentid)
        {
            try
            {
                var responseModel = await _lessonDataAccess.GetContentofAContentTypeByContentIDAsync(contentid);
                return Ok(responseModel);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/<LessonController>/5
        [HttpGet]
        [Route("GetAllContent")]
        public async Task<IActionResult> GetAllContentAsync()
        {
            try
            {
                var responseModel = await _lessonDataAccess.GetAllAvailableContentAsync();
                return Ok(responseModel);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // POST api/<LessonController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LessonController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LessonController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
