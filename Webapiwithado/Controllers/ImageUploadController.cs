using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Webapiwithado;
using Webapiwithado.DataAccess;
using Webapiwithado.Models;

namespace UploadImageWithData8777.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly UserDataAccess _userDataAccess;

        public ImageUploadController(IWebHostEnvironment environment, UserDataAccess userDataAccess)
        {
            _environment = environment;
            this._userDataAccess = userDataAccess;
        }

        [HttpPost("upload-profile-pic-user")]
        public async Task<ResponseModel> Post([FromForm] FileUploadAPI objFile)
        {
            ImageUploadResponseModel response = new ImageUploadResponseModel();
            

            try
            {
                // Check if the file is not empty
                if (objFile.files != null && objFile.files.Length > 0)
                {
                    // Create directory if it doesn't exist
                    var uploadPath = Path.Combine(_environment.WebRootPath, "Uploads");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Save the file to the server
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(objFile.files.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await objFile.files.CopyToAsync(fileStream);
                    }

                    // Update the response model
                    response.ImageName = fileName;
                    response.ImagePath = $"\\Uploads\\{fileName}";
                    response.Message = "Image uploaded successfully";

              bool imageUploaded=      await _userDataAccess.UpdateUserProfilePicture( response.ImagePath,objFile.UserId);

                    if (imageUploaded)
                    {
                        ResponseModel responseModel = new ResponseModel
                        {
                            Status = 200,
                            Message = "Image uploaded successfully",
                            Data = response
                        };

                        return responseModel;
                    }
                    else
                    {
                        ResponseModel responseModel = new ResponseModel
                        {
                            Status = 400,
                            Message = "Failed to upload image"
                        };
                        // Handle case when file is not provided
                        return responseModel;
                    }
                }
                else
                {
                    ResponseModel responseModel = new ResponseModel
                    {
                        Status = 400,
                        Message = "File not provided"
                    };
                    // Handle case when file is not provided
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                ResponseModel responseModel = new ResponseModel
                {
                    Status = 500,
                    Message = $"Failed to upload image: {ex.Message}"
                };
                // Return an error response
               return responseModel;
            }

           
        }

    }
}
