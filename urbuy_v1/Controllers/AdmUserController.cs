using urbuy_v1.Data;
using urbuy_v1.Models;
using urbuy_v1.DTOs;
using static urbuy_v1.DTOs.LoginDTO;

using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

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
                // Procure o usuário pelo nome de usuário
                var user = await _context.AdmUsers.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    return BadRequest("Nome de usuário ou senha incorretos.");
                }

                // Verifique a senha
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    return BadRequest("Nome de usuário ou senha incorretos.");
                }

                // Autenticação bem-sucedida, gere um token JWT
                var token = GenerateToken(user);

                return Ok(new { Token = token });
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
                // Verifique se o usuário já existe
                var existingUser = await _context.AdmUsers.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (existingUser != null)
                {
                    return BadRequest("Nome de usuário já em uso.");
                }

                // Hash da senha
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Crie um novo usuário
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

        private string GenerateToken(AdmUser admUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(type:ClaimTypes.NameIdentifier, value: admUser.Id.ToString()),
                    new Claim(type:ClaimTypes.Name, value: admUser.Username),
                    new Claim(type:ClaimTypes.Email, value: admUser.Email),
                    new Claim(type:ClaimTypes.Role, value: admUser.AccessLevel),
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    algorithm: SecurityAlgorithms.HmacSha256),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
