namespace AKhderApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailWithAttachment(string to, string subject, string body, byte[] attachmentData, string attachmentName);
    }
}
