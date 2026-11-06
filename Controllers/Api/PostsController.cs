using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyWithChatgpt.Models;
using StudyWithChatgpt.Services;

namespace StudyWithChatgpt.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/posts")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppLocalizer _l;

        public PostsController(AppDbContext db, UserManager<ApplicationUser> userManager, AppLocalizer localizer)
        {
            _db = db;
            _userManager = userManager;
            _l = localizer;
        }

        [HttpPost("{id:int}/like")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var postExists = await _db.BlogPosts.AnyAsync(p => p.Id == id);
            if (!postExists)
                return NotFound();

            var existing = await _db.PostLikes.FirstOrDefaultAsync(l => l.BlogPostId == id && l.UserId == userId);
            var liked = existing is null;

            try
            {
                if (existing is not null)
                    _db.PostLikes.Remove(existing);
                else
                    _db.PostLikes.Add(new PostLike { BlogPostId = id, UserId = userId, LikedAt = DateTime.Now });

                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // A concurrent request already created the like; treat it as liked.
                liked = true;
            }

            var count = await _db.PostLikes.CountAsync(l => l.BlogPostId == id);
            return Ok(new { liked, count, label = liked ? _l["post.liked"] : _l["post.like"] });
        }
    }
}
