namespace uwu.DTOs.Posts.UpdatePosts
{
    public class UpdatePostResponse
    {
        public int PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string Message { get; set; } = "Post actualizado correctamente";
    }
}
