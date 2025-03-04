namespace SmartCartCarbonFootprintApi.DTOs.UserDtos
{
    //readonly
    public class GetUserProfileDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageFileName { get; set; } 
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
