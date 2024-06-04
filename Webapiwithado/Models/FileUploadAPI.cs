using Microsoft.AspNetCore.Mvc;
using Webapiwithado.Models;

namespace Webapiwithado
{
    public class FileUploadAPI
    {
        [FromForm]
        public int UserId { get; set; }

        [FromForm]
        public IFormFile? files { get; set; }
    }

}



public class ImageUploadResponseModel
    {
        public string ImageName { get; set; }
        public string ImagePath { get; set; }

        public string Message { get; set; }

        
    }

