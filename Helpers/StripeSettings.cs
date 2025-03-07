namespace SmartCartCarbonFootprintApi.Helpers
{
    public class StripeSettings
    {
        public string Publishablekey { get; set; }
        public string Secretkey { get; set; }
        public string WebhookSecret { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }

    }
}
