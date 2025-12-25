namespace uwu.DTOs.Users.UpdateUsers
{
    public class UpdateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
