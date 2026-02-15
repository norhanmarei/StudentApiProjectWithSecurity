using Microsoft.AspNetCore.Mvc;
using StudentApi.Models;
using StudentApi.DTOs.Auth;
using StudentApi.DataSimulation;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;


namespace StudentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static string _GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var student = StudentDataSimulation.StudentsList
                .FirstOrDefault(s => s.Email == request.Email);

            if (student == null)
                return Unauthorized("Invalid credentials");

            bool isValidPassword =
                BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash);

            if (!isValidPassword)
                return Unauthorized("Invalid credentials");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                new Claim(ClaimTypes.Email, student.Email),
                new Claim(ClaimTypes.Role, student.Role)
            };
            var keyString = Environment.GetEnvironmentVariable("STUDENTAPI_DEV_JWT_KEY");

            if (string.IsNullOrEmpty(keyString))
            {
                throw new Exception("JWT secret key not found in environment variables.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "StudentApi",
                audience: "StudentApiUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = _GenerateRefreshToken();
            student.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            student.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            student.RefreshTokenRevokedAt = null;


            return Ok(new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Email == request.Email);
            if (student == null) return Unauthorized("Invalid Refresh Request");
            if (student.RefreshTokenRevokedAt != null) return Unauthorized("Revoked Refresh Token.");
            if (student.RefreshTokenExpiresAt == null || student.RefreshTokenExpiresAt <= DateTime.UtcNow)
                return Unauthorized("Expired Refresh Token.");
            bool isRefreshTokenValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, student.RefreshTokenHash);
            if (!isRefreshTokenValid) return Unauthorized("Invalid Refresh Token.");


            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                new Claim(ClaimTypes.Email, student.Email),
                new Claim(ClaimTypes.Role, student.Role)
            };
            var keyString = Environment.GetEnvironmentVariable("STUDENTAPI_DEV_JWT_KEY");

            if (string.IsNullOrEmpty(keyString))
            {
                throw new Exception("JWT secret key not found in environment variables.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "StudentApi",
                audience: "StudentApiUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                // expires: DateTime.Now.AddSeconds(20),
                signingCredentials: creds
            );
            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var newRefreshToken = _GenerateRefreshToken();
            student.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
            student.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            student.RefreshTokenRevokedAt = null;


            return Ok(new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {
            var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Email == request.Email);
            if (student == null) return Ok();
            bool isRefreshTokenValid = BCrypt.Net.BCrypt.Verify(student.RefreshTokenHash, request.RefreshToken);
            if (!isRefreshTokenValid) return Ok();
            student.RefreshTokenRevokedAt = DateTime.UtcNow;
            return Ok("Logged Out Successfully.");
        }
    }
}