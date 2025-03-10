using System.ComponentModel.DataAnnotations;

namespace BlogSystemApi.Validators
{
    public class AllowedImageFileAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly long _maxFileSize;

        public AllowedImageFileAttribute(int maxFileSizeMB)
        {
            _maxFileSize = maxFileSizeMB * 1024 * 1024; // convert MB to bytes
            ErrorMessage = $"Invalid image. Allowed extensions: {string.Join(", ", _allowedExtensions)}. Max size: {maxFileSizeMB}MB.";
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult($"Invalid file extension ({extension}). Allowed: {string.Join(", ", _allowedExtensions)}.");
                }

                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult($"File size exceeds {_maxFileSize / (1024 * 1024)}MB.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
