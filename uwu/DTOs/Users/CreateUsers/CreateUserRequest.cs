namespace uwu.DTOs.Users.CreateUsers
{
    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
