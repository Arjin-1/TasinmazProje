using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tasinmaz.DataAccess;
using Tasinmaz.Dtos;
using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthRepository(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<LoginResponseDto?> LoginAsync(UserForLoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == loginDto.Email);

            if (user == null)
                return null; 
            var attemptedHash = ComputeSha256Hash(loginDto.Password, user.Salt);

            if (attemptedHash != user.PasswordHash)
                return null; 

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Role = user.Role,
                Email = user.Email
            };
        }

        private string ComputeSha256Hash(string password, string saltBase64)
        {
            var saltBytes = Convert.FromBase64String(saltBase64);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            var combined = saltBytes.Concat(passwordBytes).ToArray();

            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(combined);

            return Convert.ToBase64String(hash);
        }

        private string GenerateSalt(int size = 64)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[size];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
        public async Task<UserDTO> RegisterAsync(UserForRegisterDto registerDto)
        {
            var exists = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == registerDto.Email);

            if (exists != null)
                throw new Exception("Email is already registered.");

            var salt = GenerateSalt();
            var hash = ComputeSha256Hash(registerDto.Password, salt);

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Salt = salt,
                PasswordHash = hash,
                Role = registerDto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        private string GenerateJwtToken(User user)
        {
            var secret = _config["JwtSettings:SecretKey"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new Exception("JwtSettings:SecretKey not found.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}

