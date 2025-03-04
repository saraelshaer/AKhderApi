using FluentValidation;
using SmartCartCarbonFootprintApi.DTOs.UserDtos;
using SmartCartCarbonFootprintApi.Models;

namespace SmartCartCarbonFootprintApi.Validators
{
    public class UserValidator : AbstractValidator<UpdateUserProfileDto>
    {
        public UserValidator()
        {
            RuleFor(user => user.FirstName)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
            
            RuleFor(user => user.LastName)
               .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
            
            RuleFor(user => user.UserName)
               .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

            RuleFor(user => user.Email)
                .EmailAddress().WithMessage("Invalid email format"); 

            RuleFor(user => user.PhoneNumber)
                .Matches(@"^\+?\d{10,15}$")
                //.When(user => !string.IsNullOrEmpty(user.PhoneNumber)) // Only validate if provided
                .WithMessage("Invalid phone number format. Must be 10-15 digits, optionally starting with +.");
        }
    }
}
