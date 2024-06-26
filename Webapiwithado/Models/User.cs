﻿namespace Webapiwithado.Models
{
    public class User
    {
        public int? UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string? Photo { get; set; }

        public int? IsVerified { get; set; }

        public String? Otp { get; set; }

        public int? SignedInWithGoogle { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? RoleId { get; set; }

        public string? RoleName { get; set; }

        public DateTime UpdatedOn { get; set; }



    }


    public class UserRegisterModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public string? Photo { get; set;}
    }



}
