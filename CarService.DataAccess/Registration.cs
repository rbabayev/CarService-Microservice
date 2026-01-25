using CarService.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarService.DataAccess
{
    public static class Registration
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CarServiceDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Default")));
        }
    }
}
