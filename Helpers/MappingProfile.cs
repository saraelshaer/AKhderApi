using AutoMapper;
using AKhderApi.backend.DTOs.DiscountDto;
using AKhderApi.DTOs.CartDtos;
using AKhderApi.DTOs.CategoryDtos;
using AKhderApi.DTOs.ProductDtos;
using AKhderApi.DTOs.UserDtos;
using AKhderApi.Models;
using AKhderApi.DTOs.ReviewDtos;
using AKhderApi.DTOs.OrderDtos;
using AKhderApi.DTOs.NotificationDtos;

namespace AKhderApi.Helpers
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

            CreateMap<ProductCart, CartItemDto>()
           .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
           .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<ProductOrder, CartItemDto>()
           .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
           .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

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

            CreateMap<User, GetUserProfileDto>();

            CreateMap<UpdateUserProfileDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ReviewDto, Review>();

            CreateMap<Review, ReadReviewDto>()
            .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserImage, opt => opt.MapFrom(src => src.User.ImageFileName))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));


            CreateMap<Order , ReadOrderDto>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.ProductOrders))
                .ReverseMap();

            CreateMap<NotificationDto, Notification>();
            CreateMap<UserNotification, ReadNotificationDto>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Notification.Message))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Notification.Title))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.NotificationId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Notification.CreatedAt))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead));
        }
    }
}
