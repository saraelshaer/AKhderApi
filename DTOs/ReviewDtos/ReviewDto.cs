using System.ComponentModel.DataAnnotations;

namespace AKhderApi.DTOs.ReviewDtos
{
    public class ReviewDto
    {

        [Required ,Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
