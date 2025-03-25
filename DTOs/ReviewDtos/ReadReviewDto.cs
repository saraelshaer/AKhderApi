using System.ComponentModel.DataAnnotations;

namespace AKhderApi.DTOs.ReviewDtos
{
    public class ReadReviewDto:ReviewDto
    {
        public int ReviewId { get; set; }
        public DateTime ReviewDate { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserImage { get; set; }
    }
}
