using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Presentation.Extensions;
using Persistence.DependencyInjection;
using Application;
using Infrastructure;
using Microsoft.OpenApi.Models;
using Domain.Models.Common;

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

// Add Infrastructure (AutoMapper, External Services, etc.)
builder.Services.AddInfrastructure();

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

// map controllers
app.MapControllers();

app.Run();