using System.ComponentModel.DataAnnotations;

namespace StudyWithChatgpt.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int BlogPostId { get; set; }

        public BlogPost? BlogPost { get; set; }

        /// <summary>
        /// Identity user that created the comment. Null for comments imported
        /// from the legacy schema.
        /// </summary>
        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(80)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
