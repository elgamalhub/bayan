namespace Bayan.Models
{
    public class PostLike
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        public int BlogPostId { get; set; }

        public BlogPost? BlogPost { get; set; }

        public DateTime LikedAt { get; set; } = DateTime.Now;
    }
}
