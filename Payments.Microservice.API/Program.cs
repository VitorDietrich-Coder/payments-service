using Games.Microservice.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Payments.Microservice.API.Extensions;
using Payments.Microservice.Application.Handlers;
using Payments.Microservice.Domain.Interfaces;
using Payments.Microservice.Infrastructure.EventStore;
using Payments.Microservice.Infrastructure.Messaging;
using Payments.Microservice.Infrastructure.Persistence;
using Serilog;
using System.Threading.Channels;
using Users.Microservice.API.Extensions;


var builder = WebApplication.CreateBuilder(args);




builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseSqlServer(connectionString),
    ServiceLifetime.Scoped);

builder.Services.AddScoped<ApplicationDbContextInitialiser>();
builder.Services.AddOpenTelemetryTracing(builder.Configuration);
builder.Services.AddApiVersioningConfiguration();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerServices();

builder.Services.AddGlobalCorsPolicy();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

 
 

builder.Services.AddHealthChecks();
 

builder.Services.AddAuthorization();

builder.Host.UseSerilog((context, services, configuration) =>
{
    SerilogExtensions.ConfigureSerilog(context, services, configuration);
});
builder.Services.AddCustomAuthentication(builder.Configuration);

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IEventStore, StoredEvents>();
builder.Services.AddScoped<GamePurchasedEventHandler>();


builder.Services.AddHostedService<GamePurchasedRabbitConsumer>();

builder.Services.AddSingleton<IEventBus>(sp =>
{
    return new RabbitMqEventBus(builder.Configuration);
});

 

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    try
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        initialiser.Initialise();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database initialisation.");

        throw;
    }

}

 
 
// Swagger
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Games API v1");
//        c.RoutePrefix = string.Empty;
//    });
//}


app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Payments API v1");

    options.RoutePrefix = string.Empty;
});

app.UseRouting();


app.UseHttpsRedirection();


app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseMiddleware<UnauthorizedResponseMiddleware>();
app.UseAuthorization();


app.MapControllers();


app.Run();

