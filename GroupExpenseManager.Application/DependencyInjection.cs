using GroupExpenseManager.Application.Expenses;
using GroupExpenseManager.Application.Groups;
using GroupExpenseManager.Application.Users;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GroupExpenseManager.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            // Register Application Services
            services.AddScoped<IGroupAppService, GroupAppService>();
            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<IExpenseAppService, ExpenseAppService>();

            return services;
        }
    }
}
