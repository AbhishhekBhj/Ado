using Webapiwithado.Models;

namespace Webapiwithado
{
    public class FileUploadAPI
    {
        public int ImgID { get; set; }
        public string ImgName { get; set; }
        public IFormFile? files { get; set; }
        public string? Users { get; set; }
    }

    public class common
    {
        public FileUploadAPI fileUploadAPI { get; set; }
        public User user { get; set; }
        public List<User> ListUser { get; set; }
    }
}
