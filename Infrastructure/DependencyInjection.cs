using Application.Repositories.Base;
using Application.Services.Base;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using Infrastructure.Extensions.RabbitMQ;
using Application.Services;
using RabbitMQ.Client;
using Domain.Models.Common;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<Infrastructure.Extensions.RabbitMQ.RabbitMqConnection>(sp =>
                RabbitMqConnection.CreateAsync(sp.GetRequiredService<IOptions<RabbitMqConfig>>().Value)
                    .GetAwaiter().GetResult());

            services.AddHostedService<RabbitMqConsumer>();

            services.AddScoped<IRabbitMqService, RabbitMqService>();
            // Register AutoMapper với tất cả Profiles trong Infrastructure assembly
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register IAutoMapper → MappingService
            services.AddScoped<IAutoMapper, MappingService>();
            services.AddScoped<IHashPassword, HashingService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ILoanInterestRate, LoanInterestRateService>();
            services.AddScoped<IDateTimeService, DateTimeService>();
            return services;
        }
    }
}