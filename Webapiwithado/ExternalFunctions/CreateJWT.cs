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

        public Dictionary<string, string>? CreateJWTToken(UserLogin userLogin)
        {
            if (userLogin != null)
            {
                // Existing access token generation logic...
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = System.Text.Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(
                    new Claim[]
                    {
          new Claim(ClaimTypes.Name, userLogin.UserName),
                    }
                  ),
                    Expires = DateTime.UtcNow.AddMinutes(15),
                    SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                string tokenString = tokenHandler.WriteToken(token);

                // Generate refresh token
                string refreshToken = GenerateRefreshToken(userLogin);

                // Create and return a dictionary with tokens
                return new Dictionary<string, string>()
                {
                    ["accessToken"] = tokenString,
                    ["refreshToken"] = refreshToken
                };
            }

            return null;
        }

        public string GenerateRefreshToken(UserLogin userLogin)
        {
            // 1. Set longer expiry for refresh token
            var refreshExpires = DateTime.UtcNow.AddDays(30); // Adjust expiry as needed

            // 2. Include additional claims specific to refresh token
            var refreshClaims = new Claim[]
            {
        new Claim(ClaimTypes.Name, userLogin.UserName),
        // Add a unique identifier for the refresh token itself
        new Claim("jti", Guid.NewGuid().ToString()) // Replace with your logic for unique identifier
            };

            // 3. Implement secure storage and management for refresh tokens (not shown here)
            // ... store the refresh token with additional info like creation time, user association etc.

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(refreshClaims),
                Expires = refreshExpires,
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!)),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var refreshToken = tokenHandler.CreateToken(tokenDescriptor);
            string refreshTokenString = tokenHandler.WriteToken(refreshToken);

            return refreshTokenString;
        }

        // Placeholder for refresh token validation and exchange endpoint
        //public string ValidateAndExchangeRefreshToken(string refreshToken)
        //{
        //    // 1. Implement logic to validate the refresh token (expiry, association with user)
        //    // ... check validity in secure storage

        //    // 2. Upon successful validation, generate a new access token
        //    var userLogin = // Retrieve user info based on refresh token validation (replace with your logic)
        //    string newAccessToken = CreateJWTToken(userLogin).accessToken; // Call existing method

        //    return newAccessToken;
        //}
    }
}
