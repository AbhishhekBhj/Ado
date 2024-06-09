namespace Webapiwithado.Models
{
    public class UserLogin
    {

        public string UserName { get; set; }

        public string Password { get; set; }
    }


    public class UserLoginGoogle
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
