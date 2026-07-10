using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlServerChallenges.Core.Data;

namespace SqlServerChallenges.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCore(this IServiceCollection  services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionStrings = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionStrings);
        });
        
        return services;
    }
}