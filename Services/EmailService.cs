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
                    _config["Email:Username"],
                    _config["Email:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(fromEmail, to, subject, body)
            {
                IsBodyHtml = false
            };

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailWithAttachment(string to, string subject, string body, byte[] attachmentData, string attachmentName)
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

            using var smtpClient = new SmtpClient(_config["Email:SmtpServer"])
            {
                Port = int.Parse(_config["Email:Port"]),
                Credentials = new NetworkCredential(
                    _config["Email:Username"],
                    _config["Email:Password"]),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage(fromEmail, to, subject, body)
            {
                IsBodyHtml = true
            };

            // Attach the PDF file if provided
            if (attachmentData != null && attachmentName != null)
            {
                var attachmentStream = new MemoryStream(attachmentData);
                mailMessage.Attachments.Add(new Attachment(attachmentStream, attachmentName, "application/pdf"));
            }

            await smtpClient.SendMailAsync(mailMessage);
        }

    }
}
