using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Auth;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interfaces;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly OrderManagementDbContext _context;
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly IUserRepository _userRepo;

        public UserController(OrderManagementDbContext context, JwtTokenGenerator TokenGenerator, IUserRepository userRepo)
        {
            _context = context;
            _tokenGenerator = TokenGenerator;
            _userRepo = userRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existingUser = await _userRepo.GetUserByUsernameAsync(dto.Username);

            if (existingUser != null)
                return BadRequest("Username already exists.");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                Role = dto.Role
            };

            await _userRepo.AddUserAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userRepo.GetUserByUsernameAsync(dto.Username);

            if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash!))
                return Unauthorized("Invalid username or password.");

            var token = _tokenGenerator.GenerateToken(user);

            return Ok(new { Token = token });
        }
    }
}
