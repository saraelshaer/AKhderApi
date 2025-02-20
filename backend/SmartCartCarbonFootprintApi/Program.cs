
using Microsoft.EntityFrameworkCore;
using SmartCartCarbonFootprintApi.Context;
using Microsoft.EntityFrameworkCore;
using SmartCartCarbonFootprintApi.Context;
using SmartCartCarbonFootprintApi.Repositories;
using SmartCartCarbonFootprintApi.Helpers;
using Microsoft.AspNetCore.Identity;
using SmartCartCarbonFootprintApi.Models;

namespace SmartCartCarbonFootprintApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); ;



            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
