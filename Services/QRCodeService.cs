using System.Drawing.Imaging;
using System.Drawing;
using QRCoder;

namespace SmartCartCarbonFootprintApi.Services
{
    public class QRCodeService
    {
        private readonly IWebHostEnvironment _env;

        public QRCodeService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string GenerateQRCode(string url, string productId)
        {
            string folderPath = Path.Combine(_env.WebRootPath, "qrcodes");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"product_{productId}.png";
            string filePath = Path.Combine(folderPath, fileName);

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
            {
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20); // Generates PNG as byte array

                File.WriteAllBytes(filePath, qrCodeBytes); // Save as PNG file
            }

            return $"/qrcodes/{fileName}"; // Return relative path for access
        }
    }
}
