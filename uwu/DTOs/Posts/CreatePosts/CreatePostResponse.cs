namespace uwu.DTOs.Posts.CreatePosts
{
    public class CreatePostResponse
    {
        public int PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string Message { get; set; } = "Post creado correctamente";
    }
}
