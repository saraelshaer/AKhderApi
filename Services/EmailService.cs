using System.Net.Mail;
using System.Net;

namespace AKhderApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            string fromEmail = _config["Email:FromAddress"];
            if (string.IsNullOrWhiteSpace(to) || !to.Contains("@"))
            {
                throw new ArgumentException("Invalid recipient email address.");
            }
            if (string.IsNullOrWhiteSpace(fromEmail) || !fromEmail.Contains("@"))
            {
                throw new Exception("Invalid sender email address.");
            }

            var smtpClient = new SmtpClient(_config["Email:SmtpServer"])
            {
                Port = int.Parse(_config["Email:Port"]),
                Credentials = new NetworkCredential(
                   Environment.GetEnvironmentVariable("EMAIL_USERNAME") ?? _config["Email:Username"],
                   Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? _config["Email:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(fromEmail, to, subject, body)
            {
                IsBodyHtml = false
            };

            await smtpClient.SendMailAsync(mailMessage);
        }


    }
}
