namespace Tasinmaz.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}

