using AKhderApi.Models;

namespace AKhderApi.backend.DTOs.SharedDto
{
    public class PaginationDto<T>
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;



        public IEnumerable<T> PaginationList { get; set; } = new List<T>();

    }
}
