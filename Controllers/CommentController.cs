using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyWithChatgpt.Models;
using StudyWithChatgpt.Services;
using StudyWithChatgpt.ViewModels;

namespace StudyWithChatgpt.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppLocalizer _l;

        public CommentController(AppDbContext db, UserManager<ApplicationUser> userManager, AppLocalizer localizer)
        {
            _db = db;
            _userManager = userManager;
            _l = localizer;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CommentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault() ?? _l["toast.error"];
                return RedirectToAction("Details", "BlogPost", new { id = model.BlogPostId });
            }

            var postExists = await _db.BlogPosts.AnyAsync(p => p.Id == model.BlogPostId);
            if (!postExists)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Challenge();

            var comment = new Comment
            {
                BlogPostId = model.BlogPostId,
                UserId = user.Id,
                Author = user.DisplayName,
                Content = model.Content.Trim(),
                CreatedAt = DateTime.Now
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            TempData["Success"] = _l["toast.commentAdded"];
            return RedirectToAction("Details", "BlogPost", new { id = model.BlogPostId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int blogPostId, string content)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment is null)
                return NotFound();

            if (!await CanManageAsync(comment))
                return Forbid();

            content = content?.Trim() ?? string.Empty;
            if (content.Length is < 1 or > 1000)
            {
                TempData["Error"] = content.Length > 1000 ? _l["val.commentMax"] : _l["val.commentRequired"];
                return RedirectToAction("Details", "BlogPost", new { id = comment.BlogPostId });
            }

            comment.Content = content;
            await _db.SaveChangesAsync();

            TempData["Success"] = _l["toast.commentUpdated"];
            return RedirectToAction("Details", "BlogPost", new { id = comment.BlogPostId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment is null)
                return NotFound();

            if (!await CanManageAsync(comment))
                return Forbid();

            var postId = comment.BlogPostId;
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();

            TempData["Success"] = _l["toast.commentDeleted"];
            return RedirectToAction("Details", "BlogPost", new { id = postId });
        }

        private async Task<bool> CanManageAsync(Comment comment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return false;

            if (User.IsInRole("Admin"))
                return true;

            if (!string.IsNullOrEmpty(comment.UserId))
                return comment.UserId == user.Id;

            // Legacy comments were stored with an author name only.
            return string.Equals(comment.Author, user.UserName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
