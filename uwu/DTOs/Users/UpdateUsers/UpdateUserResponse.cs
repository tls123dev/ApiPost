namespace uwu.DTOs.Users.UpdateUsers
{
    public class UpdateUserResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = "Usuario actualizado correctamente";
    }
}
