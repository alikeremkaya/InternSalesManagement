

using Ekip2.Application.Services.AdvanceServices;
using Ekip2.Application.Services.CompanyServices;
using Ekip2.Application.Services.LeaveService;
using Ekip2.Application.Services.LeaveTypeServices;



namespace Ekip2.Application.Extentions;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IManagerService, ManagerService>();
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ILeaveTypeService, LeaveTypeService>();
        services.AddScoped<IAdvanceService, AdvanceService>();
        services.AddScoped<ILeaveService, LeaveService>();
       

        return services;
    }
}
