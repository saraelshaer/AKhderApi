namespace BlogSystemApi.Helpers
{
    public class ImageHelper
    {
        public static string SaveImage(IFormFile imageFile, string folderName, IWebHostEnvironment _webHostEnvironment )
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string fullPath = Path.Combine(uploadsFolder, fileName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                // Copy the uploaded image to the server
                imageFile.CopyTo(fileStream);
            }
            var relativePath = Path.Combine("postImages", fileName);
            return "/" + relativePath.Replace("\\", "/");

        }

    }
}
