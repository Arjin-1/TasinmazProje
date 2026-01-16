namespace Tasinmaz.Dtos
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!; 
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
