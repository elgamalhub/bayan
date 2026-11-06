using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyWithChatgpt.Models;
using StudyWithChatgpt.Services;
using StudyWithChatgpt.ViewModels;

namespace StudyWithChatgpt.Controllers
{
    public class BlogPostController : Controller
    {
        private const int PageSize = 9;

        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageService _images;
        private readonly AppLocalizer _l;
        private readonly ILogger<BlogPostController> _logger;

        public BlogPostController(
            AppDbContext db,
            UserManager<ApplicationUser> userManager,
            IImageService images,
            AppLocalizer localizer,
            ILogger<BlogPostController> logger)
        {
            _db = db;
            _userManager = userManager;
            _images = images;
            _l = localizer;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int? category = null, string? search = null, string? sort = null)
        {
            var user = await _userManager.GetUserAsync(User);
            search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
            if (search?.Length > 100) search = search[..100];

            sort = sort?.ToLowerInvariant() is "oldest" or "liked" ? sort.ToLowerInvariant() : "newest";

            var query = _db.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .AsQueryable();

            if (category.HasValue)
                query = query.Where(p => p.CategoryId == category.Value);

            if (search is not null)
                query = query.Where(p => p.Title.Contains(search) || p.Content.Contains(search));

            query = sort switch
            {
                "oldest" => query.OrderBy(p => p.CreatedAt),
                "liked" => query.OrderByDescending(p => p.Likes.Count).ThenByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var total = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)PageSize);
            if (totalPages > 0)
                page = Math.Clamp(page, 1, totalPages);
            else
                page = 1;

            var posts = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            await ApplyLikeStateAsync(posts, user?.Id);

            var model = new BlogIndexViewModel
            {
                Posts = posts,
                Page = page,
                PageSize = PageSize,
                TotalCount = total,
                TotalPages = totalPages,
                CategoryId = category,
                Search = search,
                Sort = sort,
                Categories = await _db.Categories
                    .AsNoTracking()
                    .Select(c => new CategorySummary { Category = c, PostCount = c.BlogPosts.Count })
                    .OrderByDescending(c => c.PostCount)
                    .ThenBy(c => c.Category.Name)
                    .ToListAsync(),
                Popular = await _db.BlogPosts
                    .AsNoTracking()
                    .Include(p => p.Likes)
                    .Include(p => p.User)
                    .OrderByDescending(p => p.Likes.Count)
                    .ThenByDescending(p => p.CreatedAt)
                    .Take(4)
                    .ToListAsync(),
                ActiveCategoryName = category.HasValue
                    ? await _db.Categories.Where(c => c.Id == category.Value).Select(c => c.Name).FirstOrDefaultAsync()
                    : null
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Search(string? search) =>
            RedirectToAction(nameof(Index), new { search });

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var post = await _db.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
                return NotFound();

            post.IsLikedByCurrentUser = user is not null && post.Likes.Any(l => l.UserId == user.Id);
            post.Comments = post.Comments.OrderBy(c => c.CreatedAt).ToList();

            var related = await _db.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Where(p => p.Id != post.Id && p.CategoryId == post.CategoryId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .ToListAsync();

            ViewData["Title"] = post.Title;
            ViewData["Description"] = ArticleContent.Excerpt(post.Content, 155);
            ViewData["OgImage"] = _images.Url("PostImages", post.ImageUrl);
            ViewData["OgType"] = "article";

            return View(new PostDetailsViewModel
            {
                Post = post,
                Related = related,
                CanManage = CanManage(post, user),
                LikeCount = post.Likes.Count
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new PostFormViewModel
            {
                Categories = await CategoryOptionsAsync()
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(8 * 1024 * 1024)]
        public async Task<IActionResult> Create(PostFormViewModel model)
        {
            await ValidatePostAsync(model);

            if (!ModelState.IsValid)
            {
                model.Categories = await CategoryOptionsAsync();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Challenge();

            string? coverName = null;
            if (model.Cover is not null)
            {
                var upload = _images.Save(model.Cover, "PostImages");
                if (!upload.Ok)
                {
                    ModelState.AddModelError(nameof(model.Cover), upload.Error ?? _l["toast.error"]);
                    model.Categories = await CategoryOptionsAsync();
                    return View(model);
                }

                coverName = upload.FileName;
            }

            var post = new BlogPost
            {
                Title = model.Title.Trim(),
                Content = ArticleContent.Sanitize(model.Content),
                CategoryId = model.CategoryId,
                UserId = user.Id,
                ImageUrl = coverName,
                CreatedAt = DateTime.Now
            };

            _db.BlogPosts.Add(post);
            await _db.SaveChangesAsync();

            TempData["Success"] = _l["toast.articlePublished"];
            return RedirectToAction(nameof(Details), new { id = post.Id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var post = await _db.BlogPosts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
                return NotFound();

            if (!CanManage(post, user))
                return Forbid();

            return View(new PostFormViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CategoryId = post.CategoryId,
                ExistingImageUrl = post.ImageUrl,
                Categories = await CategoryOptionsAsync()
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(8 * 1024 * 1024)]
        public async Task<IActionResult> Edit(int id, PostFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var user = await _userManager.GetUserAsync(User);
            var post = await _db.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
                return NotFound();

            if (!CanManage(post, user))
                return Forbid();

            await ValidatePostAsync(model);

            if (!ModelState.IsValid)
            {
                model.ExistingImageUrl = post.ImageUrl;
                model.Categories = await CategoryOptionsAsync();
                return View(model);
            }

            var oldCover = post.ImageUrl;
            var newCover = oldCover;

            if (model.Cover is not null)
            {
                var upload = _images.Save(model.Cover, "PostImages");
                if (!upload.Ok)
                {
                    ModelState.AddModelError(nameof(model.Cover), upload.Error ?? _l["toast.error"]);
                    model.ExistingImageUrl = post.ImageUrl;
                    model.Categories = await CategoryOptionsAsync();
                    return View(model);
                }

                newCover = upload.FileName;
            }

            if (model.Cover is null && model.RemoveCover)
                newCover = null;

            post.Title = model.Title.Trim();
            post.Content = ArticleContent.Sanitize(model.Content);
            post.CategoryId = model.CategoryId;
            post.ImageUrl = newCover;
            post.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            if ((model.Cover is not null || model.RemoveCover) && !string.IsNullOrWhiteSpace(oldCover))
                _images.Delete("PostImages", oldCover);

            TempData["Success"] = _l["toast.articleUpdated"];
            return RedirectToAction(nameof(Details), new { id = post.Id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var post = await _db.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
                return NotFound();

            if (!CanManage(post, user))
                return Forbid();

            ViewData["Title"] = _l["post.deleteTitle"];
            return View(post);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var post = await _db.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
                return NotFound();

            if (!CanManage(post, user))
                return Forbid();

            _db.BlogPosts.Remove(post);
            await _db.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(post.ImageUrl))
                _images.Delete("PostImages", post.ImageUrl);

            TempData["Success"] = _l["toast.articleDeleted"];
            return RedirectToAction(nameof(MyPosts));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyPosts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Challenge();

            var posts = await _db.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewData["Title"] = _l["my.title"];
            return View(posts);
        }

        private bool CanManage(BlogPost post, ApplicationUser? user)
        {
            return user is not null && (post.UserId == user.Id || User.IsInRole("Admin"));
        }

        private async Task ValidatePostAsync(PostFormViewModel model)
        {
            model.Title = model.Title?.Trim() ?? string.Empty;

            if (ArticleContent.PlainText(model.Content).Length < 50)
                ModelState.AddModelError(nameof(model.Content), _l["val.contentMin"]);

            if (model.CategoryId > 0 && !await _db.Categories.AnyAsync(c => c.Id == model.CategoryId))
                ModelState.AddModelError(nameof(model.CategoryId), _l["editor.categoryRequired"]);
        }

        private async Task<List<SelectListItem>> CategoryOptionsAsync() =>
            await _db.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToListAsync();

        private async Task ApplyLikeStateAsync(IEnumerable<BlogPost> posts, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            foreach (var post in posts)
                post.IsLikedByCurrentUser = post.Likes.Any(l => l.UserId == userId);
        }
    }
}
