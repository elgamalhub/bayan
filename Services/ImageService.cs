namespace StudyWithChatgpt.Services
{
    public interface IImageService
    {
        (bool Ok, string? Error, string? FileName) Save(IFormFile file, string folder);
        bool Delete(string folder, string? fileName);
        string? Url(string folder, string? fileName);
    }

    public class ImageService : IImageService
    {
        private const long MaxBytes = 5 * 1024 * 1024;

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

        private static readonly HashSet<string> AllowedFolders = new(StringComparer.OrdinalIgnoreCase)
        {
            "PostImages", "UserImages"
        };

        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IWebHostEnvironment environment, ILogger<ImageService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public (bool Ok, string? Error, string? FileName) Save(IFormFile file, string folder)
        {
            if (file is null || file.Length == 0)
                return (false, AppStrings.Get("val.required"), null);

            if (!AllowedFolders.Contains(folder))
                return (false, AppStrings.Get("toast.error"), null);

            if (file.Length > MaxBytes)
                return (false, AppStrings.IsArabic ? "حجم الصورة يجب ألا يزيد عن 5 ميجابايت." : "Images must be 5 MB or smaller.", null);

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension))
                return (false, AppStrings.IsArabic ? "الصيغ المدعومة: JPG وPNG وGIF وWebP." : "Supported formats: JPG, PNG, GIF, and WebP.", null);

            if (!HasValidSignature(file))
                return (false, AppStrings.IsArabic ? "الملف ليس صورة صالحة." : "The file is not a valid image.", null);

            try
            {
                var safeExtension = extension.ToLowerInvariant();
                if (safeExtension == ".jpeg") safeExtension = ".jpg";

                var fileName = $"{Guid.NewGuid():N}{safeExtension}";
                var directory = Path.Combine(_environment.WebRootPath, folder);
                Directory.CreateDirectory(directory);

                var fullPath = Path.Combine(directory, fileName);
                using var stream = new FileStream(fullPath, FileMode.CreateNew);
                file.CopyTo(stream);

                return (true, null, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save uploaded image to {Folder}", folder);
                return (false, AppStrings.Get("toast.error"), null);
            }
        }

        public bool Delete(string folder, string? fileName)
        {
            if (!AllowedFolders.Contains(folder) || string.IsNullOrWhiteSpace(fileName))
                return false;

            var safeName = Path.GetFileName(fileName);
            if (string.IsNullOrWhiteSpace(safeName) || safeName != fileName)
                return false;

            try
            {
                var path = Path.Combine(_environment.WebRootPath, folder, safeName);
                if (!System.IO.File.Exists(path))
                    return false;

                System.IO.File.Delete(path);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not delete image {File} from {Folder}", fileName, folder);
                return false;
            }
        }

        public string? Url(string folder, string? fileName)
        {
            if (!AllowedFolders.Contains(folder) || string.IsNullOrWhiteSpace(fileName))
                return null;

            if (fileName != Path.GetFileName(fileName) || fileName.Contains("..", StringComparison.Ordinal))
                return null;

            return $"/{folder}/{Uri.EscapeDataString(fileName)}";
        }

        private static bool HasValidSignature(IFormFile file)
        {
            try
            {
                var header = new byte[12];
                using var stream = file.OpenReadStream();
                var read = stream.Read(header, 0, header.Length);
                if (read < 4)
                    return false;

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                return extension switch
                {
                    ".jpg" or ".jpeg" => header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
                    ".png" => header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47,
                    ".gif" => header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38,
                    ".webp" => read >= 12
                               && header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46
                               && header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50,
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }
    }
}
