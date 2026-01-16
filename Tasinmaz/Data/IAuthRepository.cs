using Tasinmaz.Dtos;
using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Data
{
    public interface IAuthRepository
    {
        Task<LoginResponseDto?> LoginAsync(UserForLoginDto loginDto);
        Task<UserDTO> RegisterAsync(UserForRegisterDto registerDto);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<List<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);

        Task<bool> SaveChangesAsync();
    }
}
