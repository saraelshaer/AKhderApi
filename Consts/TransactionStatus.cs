using System.Text.Json.Serialization;

namespace AKhderApi.Consts
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionStatus
    {
        Pending,
        Completed,
        Cancelled
    }
}
