namespace uwu.DTOs.Auth
{
    public class UserRequest
    {
        public string? Email { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
