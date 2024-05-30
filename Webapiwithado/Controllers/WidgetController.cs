using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webapiwithado.DataAccess;
using Webapiwithado.Models;

namespace Webapiwithado.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetController : ControllerBase
    {

        private readonly WidgetDataAcess _widgetDataAccess;

        public WidgetController(WidgetDataAcess widgetDataAccess)
        {
            _widgetDataAccess = widgetDataAccess;
        }

        [HttpGet]
        [Route("GetAllWidgets")]
        public async Task<IActionResult> GetAllWidgets()
        {
            try
            {
                var widgets = await _widgetDataAccess.GetWidgets();
                return Ok(widgets);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("InsertNewWidget")]

        public async Task<IActionResult> InsetNewWidget([FromBody] Widget widget)
        {
            try
            {
                await _widgetDataAccess.InsertWidgetAsync(widget);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        [HttpDelete]
        [Route("DeleteWidget/{widgetId}")]

        public async Task<IActionResult> DeleteWidgetAsync([FromRoute] int widgetId)
        {
            try
            {
                await _widgetDataAccess.DeleteWidgetAsync(widgetId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



    }

}
