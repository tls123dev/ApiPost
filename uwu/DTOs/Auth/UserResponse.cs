namespace uwu.DTOs.Auth
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public int Phone { get; set; }
        public string? Email { get; set; }
        public string Message { get; set; } = "Bienvenido a la web";
        public string Token { get; set; } = string.Empty;
    }
}
