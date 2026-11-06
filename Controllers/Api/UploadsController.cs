using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyWithChatgpt.Services;

namespace StudyWithChatgpt.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/uploads")]
    public class UploadsController : ControllerBase
    {
        private readonly IImageService _images;
        private readonly AppLocalizer _l;

        public UploadsController(IImageService images, AppLocalizer localizer)
        {
            _images = images;
            _l = localizer;
        }

        [HttpPost("image")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public IActionResult Image(IFormFile? file)
        {
            if (file is null)
                return BadRequest(new { message = _l["val.required"] });

            var upload = _images.Save(file, "PostImages");
            if (!upload.Ok)
                return BadRequest(new { message = upload.Error ?? _l["toast.error"] });

            return Ok(new
            {
                url = $"/PostImages/{Uri.EscapeDataString(upload.FileName!)}",
                name = upload.FileName
            });
        }
    }
}
