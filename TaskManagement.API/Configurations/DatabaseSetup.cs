using Microsoft.EntityFrameworkCore;
using TaskManagement.Infra.Context;

namespace TaskManagement.API.Configurations
{
    public static class DatabaseSetup
    {
        public static void AddDatabaseSetup(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            string connString = configuration.GetConnectionString("TaskManagmentConnection");

            services.AddDbContext<TaskManagmentDbContext>(options =>
            {
                options.UseSqlServer(connString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                });
            });
        }
    }
}
