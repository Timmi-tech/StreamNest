using Serilog;
using StreamNest.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.AspNetCore.Identity; 
using Microsoft.IdentityModel.Tokens; 
using StreamNest.Domain.Contracts;
using StreamNest.Infrastructure.LoggerService;
using StreamNest.API.ActionFilters;
using System.Text.Json.Serialization;
using DotNetEnv;
using StreamNest.Domain.Entities.ConfigurationsModels;

Env.Load();

var builder = WebApplication.CreateBuilder(args);



builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Host.ConfigureSerilogService();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigurePostGressContext(builder.Configuration);
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddCloudinaryConfiguration(builder.Configuration);
builder.Services.ConfigureVideoService();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger(); 

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>(); 
app.ConfigureExceptionHandler(logger); 

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StreamNest.API v1"));
app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization(); 


app.MapControllers();

app.Run();
