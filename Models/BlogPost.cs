using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bayan.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();

        /// <summary>
        /// Presentation-only flag. Never persisted.
        /// </summary>
        [NotMapped]
        public bool IsLikedByCurrentUser { get; set; }
    }
}
