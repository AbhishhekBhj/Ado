namespace Webapiwithado.Models
{
    public class ResponseModel
    {
        public int Status { get; set; }
        public string Message { get; set; }

        public dynamic? Data { get; set; }


    }
}
