using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bayan.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

        public string? ProfileImagePath { get; set; }

        public string? Bio { get; set; }

        public string? LinkedInUrl { get; set; }

        public string? WebsiteUrl { get; set; }

        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [NotMapped]
        public string DisplayName =>
            string.IsNullOrWhiteSpace(FullName) ? (UserName ?? "Member") : FullName!;

        [NotMapped]
        public string Initials
        {
            get
            {
                var source = DisplayName.Trim();
                var parts = source.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return "?";
                if (parts.Length == 1)
                    return parts[0][..Math.Min(2, parts[0].Length)].ToUpperInvariant();

                return string.Concat(parts[0][0], parts[^1][0]).ToUpperInvariant();
            }
        }
    }
}
