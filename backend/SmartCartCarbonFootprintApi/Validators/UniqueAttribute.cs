using Microsoft.EntityFrameworkCore;
using SmartCartCarbonFootprintApi.Context;
using SmartCartCarbonFootprintApi.Models;
using SmartCartCarbonFootprintApi.Repositories;
using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.Validators
{
    public class UniqueAttribute<T> : ValidationAttribute where T : class
    {
        private readonly string _columnName;

        public UniqueAttribute(string columnName)
        {
            _columnName = columnName;
        }
        protected override  ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (AppDbContext)validationContext
           .GetService(typeof(AppDbContext));

            if (context == null)
                return new ValidationResult("DB is not available.");

            var exists =  context.Set<T>().Any(t=> EF.Property<object>(t, _columnName) == value);

            if (exists)
            {
                return new ValidationResult($"{_columnName} '{value}' already exists.");
            }

            return ValidationResult.Success;
        }
    }

}
