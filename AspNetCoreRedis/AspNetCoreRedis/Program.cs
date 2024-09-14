using AspNetCoreRedis.ActionFilter;
using AspNetCoreRedis.DbContext;
using AspNetCoreRedis.Jwt;
using AspNetCoreRedis.Models;
using AspNetCoreRedis.ServiceCollection;
using AspNetCoreRedis.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace AspNetCoreRedis;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCustomSwaggerGen();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<JwtTokenGenerator>();
        builder.Services.AddCustomJwtAuthentication(builder.Configuration);

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        
        builder.Services.AddDbContext<ProductDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("ProductConnectionString"));
        });
        
        var redisConnection = builder.Configuration.GetConnectionString("RedisConnectionString");
        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}