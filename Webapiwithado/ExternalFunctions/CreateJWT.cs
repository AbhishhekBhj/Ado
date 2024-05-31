using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Webapiwithado.Models;

namespace Webapiwithado.ExternalFunctions
{
    public class CreateJWT
    {
        private readonly IConfiguration _configuration;

        public CreateJWT(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public  string? CreateJWTToken(UserLogin userLogin)
        {
            if (userLogin != null)
            {
                // Create an instance of JwtSecurityTokenHandler
                var tokenHandler = new JwtSecurityTokenHandler();

                // Convert the JWT secret key to a byte array
                var key = System.Text.Encoding.ASCII.GetBytes(_configuration["JwtSettings:Key"]!);

                // Define the token descriptor which contains information about the token
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    // Add claims to the token, which will be part of the payload
                    Subject = new ClaimsIdentity(
                        new Claim[]
                        {
                            new Claim(ClaimTypes.Name, userLogin.UserName),
                            
                        }
                    ),

                    // Set the token's expiration time (e.g., 60 minutes from now)
                    Expires = DateTime.UtcNow.AddMinutes(15),

                    // Set the signing credentials using the secret key and HMAC SHA-256 algorithm
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                // Create the token using the token handler and the token descriptor
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Write the token to a string
                string tokenString = tokenHandler.WriteToken(token);

                // Return the token string
                return tokenString;
            }

            // Return null if userLogin is null
            return null;
        }
    }
}
