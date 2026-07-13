using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionStrings = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionStrings);
        });

        services.AddScoped<SqlConnection>(_
            => new SqlConnection(configuration.GetConnectionString("AdventureWorks"))
        );

        services.AddSingleton<MsSqlQuerySyntaxChecker>();

        services.AddScoped<IQueryExecutor, MsSqlQueryExecutor>();
        services.AddScoped<QueryExecutorDispatcher>();

        services.AddScoped<IQuerySyntaxChecker, MsSqlQuerySyntaxChecker>();
        services.AddScoped<SyntaxCheckerDispatcher>();
        
        

        return services;
    }
}