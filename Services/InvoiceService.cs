using QuestPDF.Fluent;
using QuestPDF.Helpers;
using SmartCartCarbonFootprintApi.Models;

namespace SmartCartCarbonFootprintApi.Services
{
    public class InvoiceService
    {
        public byte[] GenerateInvoice(Payment payment)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header().Text("Invoice").FontSize(20).Bold();

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Transaction ID: {payment.TransactionId}");
                        col.Item().Text($"Amount: {payment.Amount} {payment.Currency.ToUpper()}");
                        col.Item().Text($"Email: {payment.Email}");
                        col.Item().Text($"Date: {payment.PaymentDate}");
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
