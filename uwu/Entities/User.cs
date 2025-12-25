using System.ComponentModel.DataAnnotations;

namespace uwu.Entities
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string? Name { get; set; }
        public int Age { get; set; }
        public int Phone { get; set; }
        [EmailAddress]
        [Required]
        public string? Email { get; set; }
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}