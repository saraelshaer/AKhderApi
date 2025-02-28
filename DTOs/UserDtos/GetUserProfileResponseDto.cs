namespace SmartCartCarbonFootprintApi.DTOs.UserDtos
{
    public class GetUserProfileResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public GetUserProfileDto? User { get; set; }
    }
}
