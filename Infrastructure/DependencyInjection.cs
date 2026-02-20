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
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Infrastructure.Extensions.HangFire;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var hangfireConn = configuration.GetConnectionString("BackgroundConnection");
            HangfireDatabaseEnsurer.EnsureHangfireDatabaseExists(hangfireConn ?? "");

            services.AddHangfire(sp =>
                sp.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(
                            hangfireConn,
                            new SqlServerStorageOptions
                            {
                                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                QueuePollInterval = TimeSpan.Zero,
                                UseRecommendedIsolationLevel = true,
                                DisableGlobalLocks = true
                            }));
            services.AddHangfireServer();
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

            // Register HangFire Service
            services.AddScoped<IHangFireService, HangFireService>();
            // Đăng ký recurring jobs (Clean Arch: Presentation chỉ gọi IRecurringJobRegistrar)
            services.AddSingleton<IRecurringJobRegistrar, RecurringJobRegistrar>();
            return services;
        }
    }
}