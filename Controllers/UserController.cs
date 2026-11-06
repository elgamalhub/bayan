using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyWithChatgpt.Models;
using StudyWithChatgpt.Services;
using StudyWithChatgpt.ViewModels;

namespace StudyWithChatgpt.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IImageService _images;
        private readonly AppLocalizer _l;

        public UserController(
            AppDbContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IImageService images,
            AppLocalizer localizer)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _images = images;
            _l = localizer;
        }

        [HttpGet]
        public IActionResult Signup()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Signup", model);

            if (await _userManager.FindByNameAsync(model.Username) is not null)
            {
                ModelState.AddModelError(nameof(model.Username), _l["auth.duplicateUser"]);
                return View("Signup", model);
            }

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                ModelState.AddModelError(nameof(model.Email), _l["auth.duplicateEmail"]);
                return View("Signup", model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Username.Trim(),
                Email = model.Email.Trim(),
                CreatedDate = DateTime.Now,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View("Signup", model);
            }

            if (await _db.Roles.AnyAsync(r => r.Name == "User"))
                await _userManager.AddToRoleAsync(user, "User");

            TempData["Success"] = _l["auth.registered"];
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var identifier = model.Username.Trim();
            var user = identifier.Contains('@')
                ? await _userManager.FindByEmailAsync(identifier)
                : await _userManager.FindByNameAsync(identifier);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, _l["auth.invalid"]);
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, _l["auth.locked"]);
                return View(model);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, _l["auth.invalid"]);
                return View(model);
            }

            TempData["Success"] = _l.Format("auth.welcome", user.DisplayName);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> PublicProfile(string? userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
                return NotFound();

            var posts = await _db.BlogPosts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewData["Title"] = user.DisplayName;
            ViewData["Description"] = user.Bio;

            return View(new PublicProfileViewModel
            {
                Username = user.UserName ?? string.Empty,
                DisplayName = user.DisplayName,
                Initials = user.Initials,
                Bio = user.Bio,
                LinkedInUrl = UrlSafety.NormalizeExternal(user.LinkedInUrl),
                WebsiteUrl = UrlSafety.NormalizeExternal(user.WebsiteUrl),
                ProfileImageUrl = _images.Url("UserImages", user.ProfileImagePath),
                JoinDate = user.CreatedDate,
                Posts = posts,
                IsOwner = string.Equals(_userManager.GetUserId(User), user.Id, StringComparison.Ordinal),
                LikeCount = posts.Sum(p => p.Likes.Count),
                CommentCount = posts.Sum(p => p.Comments.Count)
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Challenge();

            return View(new EditProfileViewModel
            {
                Username = user.UserName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Bio = user.Bio,
                LinkedInUrl = user.LinkedInUrl,
                WebsiteUrl = user.WebsiteUrl,
                ExistingImageUrl = _images.Url("UserImages", user.ProfileImagePath)
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(4 * 1024 * 1024)]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Challenge();

            model.ExistingImageUrl = _images.Url("UserImages", user.ProfileImagePath);

            if (!ModelState.IsValid)
                return View(model);

            var normalizedUsername = model.Username.Trim();
            if (!string.Equals(user.UserName, normalizedUsername, StringComparison.OrdinalIgnoreCase))
            {
                var owner = await _userManager.FindByNameAsync(normalizedUsername);
                if (owner is not null && owner.Id != user.Id)
                {
                    ModelState.AddModelError(nameof(model.Username), _l["profile.usernameTaken"]);
                    return View(model);
                }
            }

            var normalizedEmail = model.Email.Trim();
            if (!string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
            {
                var owner = await _userManager.FindByEmailAsync(normalizedEmail);
                if (owner is not null && owner.Id != user.Id)
                {
                    ModelState.AddModelError(nameof(model.Email), _l["profile.emailTaken"]);
                    return View(model);
                }
            }

            string? newImage = null;
            if (model.ProfileImage is not null)
            {
                var upload = _images.Save(model.ProfileImage, "UserImages");
                if (!upload.Ok)
                {
                    ModelState.AddModelError(nameof(model.ProfileImage), upload.Error ?? _l["profile.imageInvalid"]);
                    return View(model);
                }

                newImage = upload.FileName;
            }

            var oldImage = user.ProfileImagePath;

            user.UserName = normalizedUsername;
            user.Email = normalizedEmail;
            user.FullName = string.IsNullOrWhiteSpace(model.FullName) ? null : model.FullName.Trim();
            user.Bio = string.IsNullOrWhiteSpace(model.Bio) ? null : model.Bio.Trim();
            user.LinkedInUrl = UrlSafety.NormalizeExternal(model.LinkedInUrl);
            user.WebsiteUrl = UrlSafety.NormalizeExternal(model.WebsiteUrl);

            if (newImage is not null)
                user.ProfileImagePath = newImage;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                if (newImage is not null)
                    _images.Delete("UserImages", newImage);

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            if (newImage is not null && !string.IsNullOrWhiteSpace(oldImage))
                _images.Delete("UserImages", oldImage);

            await _db.Comments
                .Where(c => c.UserId == user.Id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.Author, user.DisplayName));

            TempData["Success"] = _l["profile.updated"];
            return RedirectToAction(nameof(PublicProfile), new { userName = user.UserName });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Challenge();

            return RedirectToAction(nameof(PublicProfile), new { userName = user.UserName });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }
}
