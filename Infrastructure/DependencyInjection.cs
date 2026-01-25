using Application.Repositories.Base;
using Application.Services.Base;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using Infrastructure.Extensions.RabbitMQ;
using Application.Services;
using Domain.Models.Common;
using StackExchange.Redis;
using Infrastructure.Extensions.Redit;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Register RabbitMQ
            services.AddSingleton<Infrastructure.Extensions.RabbitMQ.RabbitMqConnection>(sp =>
                RabbitMqConnection.CreateAsync(sp.GetRequiredService<IOptions<RabbitMqConfig>>().Value)
                    .GetAwaiter().GetResult());

            services.AddHostedService<RabbitMqConsumer>();

            // Register RabbitMQ Service
            services.AddScoped<IRabbitMqService, RabbitMqService>();

            // Register Redis
            // Register AutoMapper với tất cả Profiles trong Infrastructure assembly
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register Redis Service
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(sp.GetRequiredService<IOptions<RedisConfig>>().Value.ConnectionString));
            services.AddScoped<IRedisService, RedisService>();

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