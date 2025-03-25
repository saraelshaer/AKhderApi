using System.Text.Json.Serialization;

namespace AKhderApi.Consts
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentMethod
    {
        Stripe,
        CreditCard,         
        BankTransfer, 
        CashOnDelivery 
    }

}
