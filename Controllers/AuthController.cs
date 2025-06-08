using Microsoft.AspNetCore.Mvc;
using CareerGuidancePlatform.Data;
using CareerGuidancePlatform.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace CareerGuidancePlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public class RegisterRequest
        {
            public string Username { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest("Username cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Password cannot be empty.");

            if (request.Password != request.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            // Kiểm tra username/email đã tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Username is already taken.");

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email is already registered.");

            var user = new User
            {
                Username = request.Username,
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username and password are required.");

            var hashed = HashPassword(request.Password);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == hashed);

            if (user == null)
                return Unauthorized("Invalid credentials");

            HttpContext.Session.SetInt32("UserId", user.UserId);
            return Ok("Login successful");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logged out");
        }

        private string HashPassword(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
