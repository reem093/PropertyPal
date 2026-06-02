using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReportApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthDummyController : ControllerBase
    {
        private const string SecretKey = "8F2C9A1D7B4E6F8A2C5D9E1F3A7B6C4D";
        private const string Issuer = "PoropertyBackEndApp";
        private const string Audience = "DummyUsers";

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Dummy validation
            if (request.Username != "admin" || request.Password != "123456")
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }

            var token = GenerateToken(request.Username);
            
           // return Ok(token);

          return Ok(new
            {
                access_token = token,
                expires_in = 3600
            });
        }

        private string GenerateToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(SecretKey));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "PropertyManager")
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}