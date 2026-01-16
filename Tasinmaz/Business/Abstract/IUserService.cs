using Tasinmaz.Dtos;

namespace Tasinmaz.Business.Abstract
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserByIdAsync(int id);

        Task<List<UserDTO>> GetAllUsersAsync();

        Task<UserDTO> UpdateUserAsync(int id, UserForUpdateDto updateDto);

        Task DeleteUserAsync(int id);

        Task<UserDTO> RegisterUserAsync(UserForRegisterDto registerDto);

        Task<byte[]> ExportUsersToExcelAsync();
        Task<byte[]> ExportUsersToPdfAsync();

    }
}
