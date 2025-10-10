using AppointmetScheduler.Data;
using AppointmetScheduler.Entities;
using AppointmetScheduler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppointmetScheduler.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        public AuthServices(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<User?> RegisterAsync(RegDto request)
        {
            if(await _context.Users.AnyAsync(e => e.Email == request.Email))
            {
                return null;
            }
            User user = new User();
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            var hashed = new PasswordHasher<User>().HashPassword(user, request.Password);
            user.PasswordHash = hashed;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<string?> LoginAsync(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Email == request.Email);
            if (user == null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user,user.PasswordHash,request.Password)
                ==PasswordVerificationResult.Failed)
            {
                return null;
            }
            var result = CreateToken(user);
            return result;
        }


        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                new Claim(ClaimTypes.Name,user.Email),
                new Claim(ClaimTypes.Role,user.Role.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("AppSettings:Token")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokendiscriptor = new JwtSecurityToken(
                    issuer: _config.GetValue<string>("AppSettings:Issuer"),
                    audience: _config.GetValue<string>("AppSettings:Audience"),
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds
                );
            var token = new JwtSecurityTokenHandler().WriteToken(tokendiscriptor);
            return token;
        }
    }
}
