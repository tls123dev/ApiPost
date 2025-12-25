namespace uwu.DTOs.Users.CreateUsers
{
    public class CreateUserResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = "Bienvenido a la web";
    }
}
