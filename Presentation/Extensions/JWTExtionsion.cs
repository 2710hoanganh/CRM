using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Presentation.Extensions
{
    public static class JWTExtension
    {
        public static void AddJWTExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Validate the issuer of the token
                        ValidateIssuer = true,
                        // Validate the audience of the token
                        ValidateAudience = true,
                        // Validate the lifetime of the token
                        ValidateLifetime = true,
                        // Validate the signing key of the token
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"] ?? string.Empty,
                        ValidAudience = configuration["Jwt:Audience"] ?? string.Empty,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty))
                    };
                });
        }
    }
}