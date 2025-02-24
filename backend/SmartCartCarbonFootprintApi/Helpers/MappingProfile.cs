using AutoMapper;
using SmartCartCarbonFootprintApi.DTOs.CategoryDtos;
using SmartCartCarbonFootprintApi.DTOs.ProductDtos;
using SmartCartCarbonFootprintApi.Models;

namespace SmartCartCarbonFootprintApi.Helpers
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<Category, GetCategoryDto>();

            CreateMap<Product, ReadProductDto>()
                .ReverseMap();
            CreateMap<CreateProductDto, Product>();
        }
    }
}
