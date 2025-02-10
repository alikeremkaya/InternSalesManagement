

using Ekip2.Infrastructure.Repositories.AdvanceRepositories;
using Ekip2.Infrastructure.Repositories.CompanyRepositories;
using Ekip2.Infrastructure.Repositories.LeaveRepositories;
using Ekip2.Infrastructure.Repositories.LeaveTypeRepositories;

namespace Ekip2.Infrastructure.Extentions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(configuration.GetConnectionString("AppConnectionDev"));
            });
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IManagerRepository, ManagerRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
            services.AddScoped<IAdvanceRepository, AdvanceRepository>();
            services.AddScoped<ILeaveRepository, LeaveRepository>();

            AdminSeed.SeedAsync(configuration).GetAwaiter().GetResult();

            return services;
        }
    }
}
