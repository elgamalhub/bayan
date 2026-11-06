using StudyWithChatgpt.Models;
using StudyWithChatgpt.Services;

namespace StudyWithChatgpt.ViewModels
{
    public class HomeViewModel
    {
        public BlogPost? Featured { get; set; }
        public List<BlogPost> Latest { get; set; } = new();
        public List<BlogPost> Popular { get; set; } = new();
        public List<CategorySummary> Categories { get; set; } = new();
        public int PostCount { get; set; }
        public int WriterCount { get; set; }
        public int CategoryCount { get; set; }
    }

    public class CategorySummary
    {
        public Category Category { get; set; } = new();
        public int PostCount { get; set; }
    }

    public class BlogIndexViewModel
    {
        public List<BlogPost> Posts { get; set; } = new();
        public List<CategorySummary> Categories { get; set; } = new();
        public List<BlogPost> Popular { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int? CategoryId { get; set; }
        public string? Search { get; set; }
        public string? Sort { get; set; } = "newest";

        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
        public bool HasFilters => CategoryId.HasValue || !string.IsNullOrWhiteSpace(Search);
        public string? ActiveCategoryName { get; set; }
    }

    public class PostDetailsViewModel
    {
        public BlogPost Post { get; set; } = new();
        public List<BlogPost> Related { get; set; } = new();
        public bool CanManage { get; set; }
        public int LikeCount { get; set; }
    }

    public class PostFormViewModel
    {
        public int Id { get; set; }

        [LocalizedRequired]
        [LocalizedMaxLength(100, "val.titleMax")]
        public string Title { get; set; } = string.Empty;

        [LocalizedRequired]
        public string Content { get; set; } = string.Empty;

        [LocalizedRange(1, int.MaxValue, "editor.categoryRequired")]
        public int CategoryId { get; set; }

        public string? ExistingImageUrl { get; set; }

        public bool RemoveCover { get; set; }

        public IFormFile? Cover { get; set; }

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Categories { get; set; } = new();

        public bool IsEdit => Id > 0;
    }

    public class CommentFormViewModel
    {
        [LocalizedRange(1, int.MaxValue, "toast.error")]
        public int BlogPostId { get; set; }

        [LocalizedRequired("val.commentRequired")]
        [LocalizedMaxLength(1000, "val.commentMax")]
        public string Content { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [LocalizedRequired]
        [LocalizedRegularExpression(@"^[A-Za-z0-9\u0600-\u06FF._-]{3,32}$", "val.username")]
        public string Username { get; set; } = string.Empty;

        [LocalizedRequired]
        [LocalizedEmail]
        public string Email { get; set; } = string.Empty;

        [LocalizedRequired]
        [LocalizedMinLength(6)]
        public string Password { get; set; } = string.Empty;

        [LocalizedRequired]
        [LocalizedCompare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [LocalizedRequired]
        public string Username { get; set; } = string.Empty;

        [LocalizedRequired]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class EditProfileViewModel
    {
        [LocalizedRequired]
        [LocalizedRegularExpression(@"^[A-Za-z0-9\u0600-\u06FF._-]{3,32}$", "val.username")]
        public string Username { get; set; } = string.Empty;

        [LocalizedMaxLength(80)]
        public string? FullName { get; set; }

        [LocalizedRequired]
        [LocalizedEmail]
        public string Email { get; set; } = string.Empty;

        [LocalizedMaxLength(400)]
        public string? Bio { get; set; }

        public string? LinkedInUrl { get; set; }

        public string? WebsiteUrl { get; set; }

        public IFormFile? ProfileImage { get; set; }

        public string? ExistingImageUrl { get; set; }
    }

    public class PublicProfileViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime JoinDate { get; set; }
        public List<BlogPost> Posts { get; set; } = new();
        public bool IsOwner { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public string Initials { get; set; } = "?";
    }
}
