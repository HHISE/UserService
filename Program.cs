using System.Text;
using Scalar.AspNetCore;
using UserService.Repositories;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService.RabbitMQ;
using UserService.Services;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();
logger.Debug("start min service");

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    string mySecret = builder.Configuration["Secret"] ?? "none";
    string myIssuer = builder.Configuration["Issuer"] ?? "none";
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = myIssuer,
                ValidAudience = "http://localhost",
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mySecret))
            };
        });

// Add services to the container.


    builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.AddScoped<IUserRepository, UserRepository>();

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    
    builder.Services.AddSingleton<RabbitMQConnection>();
    builder.Services.AddSingleton<RabbitMQService>();
    
    var app = builder.Build();
    
    // Initialize RabbitMQ
    var rabbitMQService = app.Services.GetRequiredService<RabbitMQService>();

    await rabbitMQService.InitializeAsync();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //1. Lav OpenAPI endepunkt (defaults to /openapi/v1.json)
        app.MapOpenApi();
//2. Start Scalar webUI (default: /scalar/v1)
        app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
    
}

catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}

finally
{
    NLog.LogManager.Shutdown();
}