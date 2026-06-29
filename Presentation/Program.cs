using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Presentation.Extensions;
using Persistence.DependencyInjection;
using Application;
using Infrastructure;
using Microsoft.OpenApi.Models;
using Domain.Models.Common;
using Infrastructure.Extensions.RabbitMQ;
using Hangfire;
using Application.Services;
using Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers and Versioning
builder.Services.AddControllers();
// Add Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
// Add Application (MediatR, etc.)
builder.Services.AddApplication();

// Configure RabbitMQ
builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<RabbitMqConsumer>();

// Add Infrastructure (AutoMapper, External Services, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Persistence (DbContext, Repositories, UnitOfWork)
builder.Services.AddPersistence(builder.Configuration);

// Add Endpoints API Explorer
builder.Services.AddEndpointsApiExplorer();

// Add Swagger
// Define of swagger extension for reason of reusability
builder.Services.AddSwaggerExtension();

// Add JWT
builder.Services.AddJWTExtension(builder.Configuration);

builder.Services.AddSwaggerGen(c =>
{
    // JWT Auth
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.Configure<JWT>(
    builder.Configuration.GetSection("Jwt"));

var app = builder.Build();

// Đợi SQL sẵn sàng rồi mới Migrate (Docker compose: db container start trước nhưng SQL chưa listen ngay)
const int maxMigrateRetries = 15;
const int migrateRetryDelayMs = 3000;
for (var attempt = 1; attempt <= maxMigrateRetries; attempt++)
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }
        break;
    }
    catch (Exception)
    {
        if (attempt == maxMigrateRetries) throw;
        Thread.Sleep(migrateRetryDelayMs);
    }
}

//Swagger
// if is development, use swagger
if (app.Environment.IsDevelopment())
{
    // use swagger
    app.UseSwagger();
    // use swagger ui
    app.UseSwaggerUI(c =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"HCRM API {description.GroupName}");
        }
    });
}
// use https redirection
app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Hangfire (dashboard + recurring jobs qua IRecurringJobRegistrar — Clean Architecture)
app.UseHangfireDashboard("/hangfire");
app.Services.GetRequiredService<IRecurringJobRegistrar>().RegisterRecurringJobs();
// map controllers
app.MapControllers();

app.Run();