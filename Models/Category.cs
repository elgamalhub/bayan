using System.ComponentModel.DataAnnotations;

namespace Bayan.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(60)]
        public string Name { get; set; } = string.Empty;

        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
}
