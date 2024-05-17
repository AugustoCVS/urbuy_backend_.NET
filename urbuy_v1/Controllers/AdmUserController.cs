using urbuy_v1.Data;
using urbuy_v1.Models;
using urbuy_v1.DTOs;
using static urbuy_v1.DTOs.LoginDTO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace urbuy_v1.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AdmUserController : Controller
    {

        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AdmUserController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
  
                var user = await _context.AdmUsers.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    return BadRequest("Nome de usuário ou senha incorretos.");
                }

                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    return BadRequest("Nome de usuário ou senha incorretos.");
                }

                var loggedUser = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    AccessLevel = user.AccessLevel
                };

                return Ok(loggedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            try
            {
                var existingUser = await _context.AdmUsers.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (existingUser != null)
                {
                    return BadRequest("Nome de usuário já em uso.");
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var newUser = new AdmUser
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = passwordHash,
                    AccessLevel = model.AccessLevel
                };

                _context.AdmUsers.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok("Registro bem-sucedido.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string accessLevel)
        {
            try
            {
                var users = await _context.AdmUsers.ToListAsync();

                if (accessLevel == "admin")
                {
                    var adminUsers = users.Select(user => new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        AccessLevel = user.AccessLevel
                    });

                    return Ok(adminUsers);
                }
                else
                {
                    var basicUsers = users.Select(user => new UserBasicDTO
                    {
                        Username = user.Username,
                        Email = user.Email
                    });

                    return Ok(basicUsers);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }


    }
}
