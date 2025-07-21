using StreamNest.Domain.Contracts;
using StreamNest.Infrastructure.LoggerService;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Text; 
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using StreamNest.Domain.Entities.Models;
using StreamNest.Application.Services.Contracts;
using StreamNest.Application.Services;
using StreamNest.Infrastructure.Repository;
using DotNetEnv;
using StreamNest.Domain.Entities.ConfigurationsModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CloudinaryDotNet;

namespace StreamNest.Extensions

{
    public static class ServiceExtensions
    {
        public static IHostBuilder ConfigureSerilogService(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console();
            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        // Add the ConfigureCors method to the ServiceExtensions class.
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:3001", "http://localhost:3000")
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options => { });
        }

        public static void ConfigurePostGressContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<RepositoryContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
        }
        public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();
        public static void ConfigureRepositoryManager(this IServiceCollection services) => services.AddScoped<IRepositoryManager, RepositoryManager>(); 

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            Env.Load();

            var jwtConfiguration = new JwtConfiguration();
            configuration.Bind(jwtConfiguration.Section, jwtConfiguration);
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");


            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(Options =>
            {
                Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfiguration.ValidIssuer,
                    ValidAudience = jwtConfiguration.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

                };
            });
        }
        public static IServiceCollection AddCloudinaryConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudinaryConfigurations>(configuration.GetSection("Cloudinary"));

            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CloudinaryConfigurations>>().Value;
                var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
                return new Cloudinary(account);
            });
            return services;
        }
        public static void ConfigureVideoService(this IServiceCollection services)
        {
            services.AddScoped<IVideoService, VideoService>();
        }
        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));
    }
}