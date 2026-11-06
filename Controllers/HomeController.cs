using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyWithChatgpt.Models;
using StudyWithChatgpt.Services;
using StudyWithChatgpt.ViewModels;

namespace StudyWithChatgpt.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly AppLocalizer _l;

        public HomeController(AppDbContext db, AppLocalizer localizer)
        {
            _db = db;
            _l = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var postsQuery = _db.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments);

            var latest = await postsQuery
                .OrderByDescending(p => p.CreatedAt)
                .Take(7)
                .ToListAsync();

            var featured = latest.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.ImageUrl)) ?? latest.FirstOrDefault();

            var model = new HomeViewModel
            {
                Featured = featured,
                Latest = latest.Where(p => p.Id != featured?.Id).Take(4).ToList(),
                Popular = await postsQuery
                    .OrderByDescending(p => p.Likes.Count)
                    .ThenByDescending(p => p.CreatedAt)
                    .Take(4)
                    .ToListAsync(),
                Categories = await _db.Categories
                    .AsNoTracking()
                    .Select(c => new CategorySummary
                    {
                        Category = c,
                        PostCount = c.BlogPosts.Count
                    })
                    .OrderByDescending(c => c.PostCount)
                    .ThenBy(c => c.Category.Name)
                    .Take(6)
                    .ToListAsync(),
                PostCount = await _db.BlogPosts.CountAsync(),
                WriterCount = await _db.Users.CountAsync(),
                CategoryCount = await _db.Categories.CountAsync()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Privacy() => View();

        [HttpGet]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });

        [HttpGet]
        public new IActionResult StatusCode(int code)
        {
            if (code == 404)
            {
                Response.StatusCode = 404;
                return View("NotFound");
            }

            Response.StatusCode = code;
            return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }
}
