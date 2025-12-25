namespace uwu.DTOs.Posts.ReadPosts
{
    public class ReadPostResponse
    {
        public int PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
    }
}