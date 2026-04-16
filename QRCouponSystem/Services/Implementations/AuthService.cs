using Microsoft.IdentityModel.Tokens;
using QRCouponSystem.DbModels;
using QRCouponSystem.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using QRCouponSystem.DTOs;
using Microsoft.EntityFrameworkCore;

namespace QRCouponSystem.Services.Implementations
{
    public class AuthService: IAuthService
    {
        private readonly QrcouponSystemContext _context;
        private readonly IConfiguration _config;

        public AuthService(QrcouponSystemContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> RegisterAsync(AuthRequestDto dto)
        {
            var user = new User
            {
                Username = dto.UserName,
                PasswordHash = dto.UserPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _context.Wallets.Add(new Wallet { UserId = user.Id, Balance = 0 });
            await _context.SaveChangesAsync();

            return "User Registered";
        }

        public async Task<AuthResponseDto> LoginAsync(AuthRequestDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Username == dto.UserName &&
                    x.PasswordHash == dto.UserPassword);

            if (user == null)
                throw new Exception("Invalid credentials");

            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var token = new JwtSecurityToken(
                claims: new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDto
            {
                AccessToken = jwt
            };
        }
    }
}
