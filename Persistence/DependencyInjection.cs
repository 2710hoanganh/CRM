using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Persistence.Contexts;
using Persistence.Repositories.Base;
using Persistence.Repositories;
using Application.Repositories.Base;
using Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserReferenceRepository, UserReferenceRepository>();
            services.AddScoped<ILoanRepository, LoanRepository>();
            
            return services;
        }
    }
}