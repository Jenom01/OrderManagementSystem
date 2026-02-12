using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Auth;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interfaces;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtTokenGenerator _tokenGenerator;

        public UserController(IUserRepository userRepo, JwtTokenGenerator TokenGenerator)
        {
            _userRepo = userRepo;
            _tokenGenerator = TokenGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                Role = dto.Role
            };

            await _userRepo.AddUserAsync(user);
            await _userRepo.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userRepo.GetUserByUsernameAsync(dto.Username);

            if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash!))
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = _tokenGenerator.GenerateToken(user);

            return Ok(new { Token = token });
        }
    }
}
