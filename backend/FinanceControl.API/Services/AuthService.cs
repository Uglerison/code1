using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FinanceControl.API.Models;
using FinanceControl.API.Data;

namespace FinanceControl.API.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly FinanceDbContext _context;

    public AuthService(IConfiguration configuration, FinanceDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public async Task<User?> ValidateUser(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return null;

        bool validPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        return validPassword ? user : null;
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"] ?? "your-super-secret-key-with-at-least-32-characters");
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
} 