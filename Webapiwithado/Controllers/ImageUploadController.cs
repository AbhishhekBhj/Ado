using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Webapiwithado;
using Webapiwithado.Models;

namespace UploadImageWithData8777.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ImageUploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public async Task<ActionResult<common>> Post([FromForm] FileUploadAPI objFile)
        {
            var obj = new common();
            obj.ListUser = new List<User>();
            obj.fileUploadAPI = new FileUploadAPI();

            try
            {
                // Deserialize the JSON string to a list of users
                if (!string.IsNullOrEmpty(objFile.Users))
                {
                    obj.ListUser = JsonConvert.DeserializeObject<List<User>>(objFile.Users);
                }

                // Set image ID and name
                obj.fileUploadAPI.ImgID = objFile.ImgID;
                obj.fileUploadAPI.ImgName = $"\\Upload\\{objFile.files.FileName}";

                // Check if the file is not empty
                if (objFile.files != null && objFile.files.Length > 0)
                {
                    // Create directory if it doesn't exist
                    var uploadPath = Path.Combine(_environment.WebRootPath, "Upload");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Save the file to the server
                    var filePath = Path.Combine(uploadPath, objFile.files.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await objFile.files.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                // Return an error response
                return BadRequest(new { message = ex.Message });
            }

            // Return the common object with the details
            return Ok(obj);
        }
    }
}
