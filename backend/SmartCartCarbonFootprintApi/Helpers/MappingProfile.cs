using AutoMapper;
using SmartCartCarbonFootprintApi.DTOs.CategoryDtos;
using SmartCartCarbonFootprintApi.DTOs.DiscountDto;
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
                .ForMember(dest => dest.DiscountPercentage , src => src.MapFrom(src=>src.Discount.Percentage))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src =>
                   src.DiscountId != null && src.Discount != null && src.Discount.ExpiryDate >= DateOnly.FromDateTime(DateTime.Now)
                    ? Math.Round(src.Price * (1 - src.Discount.Percentage / 100), 2) 
                    : src.Price
                 ))
                .ReverseMap();

            CreateMap<CreateProductDto, Product>();

            CreateMap<UpdateProductDto, Product>()
                 .ForMember(dest => dest.Price, opt => opt.PreCondition(src => src.Price.HasValue))
                 .ForMember(dest => dest.CarbonFootprint, opt => opt.PreCondition(src => src.CarbonFootprint.HasValue))
                 .ForMember(dest => dest.StockQuantity, opt => opt.PreCondition(src => src.StockQuantity.HasValue))
                 .ForMember(dest => dest.CategoryId, opt => opt.PreCondition(src => src.CategoryId.HasValue))
                 .ForMember(dest => dest.DiscountId, opt => opt.PreCondition(src => src.DiscountId.HasValue))
                 .ForAllMembers(opts => opts.Condition((src,dest, srcMember) => srcMember != null));

            CreateMap<DiscountDto, Discount>();

            CreateMap<Discount, ReadDiscountDto>();
        }
    }
}
