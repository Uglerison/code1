using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using FinanceControl.API.Models;
using FinanceControl.API.Models.DTOs;
using FinanceControl.API.Services;
using FinanceControl.API.Data;
using Microsoft.Extensions.Logging;

namespace FinanceControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly FinanceDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AuthService authService, 
            FinanceDbContext context,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                {
                    return BadRequest("Email e senha são obrigatórios");
                }

                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

                if (existingUser != null)
                {
                    return BadRequest("Email já está em uso");
                }

                var user = new User
                {
                    Email = dto.Email,
                    Name = dto.Name ?? dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Usuário registrado com sucesso: {user.Email}");

                return Ok(new
                {
                    user.Email,
                    user.Name,
                    Token = _authService.GenerateToken(user)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário");
                return StatusCode(500, "Erro interno ao processar o registro");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto dto)
        {
            try
            {
                var user = await _authService.ValidateUser(dto.Email, dto.Password);
                if (user == null)
                {
                    return Unauthorized("Email ou senha inválidos");
                }

                return Ok(new
                {
                    user.Email,
                    user.Name,
                    Token = _authService.GenerateToken(user)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer login");
                return StatusCode(500, "Erro interno ao processar o login");
            }
        }
    }
} 