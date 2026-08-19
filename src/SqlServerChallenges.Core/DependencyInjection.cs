using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlServerChallenges.Core.Common.Behaviours;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.QueryExecutor;
using SqlServerChallenges.Core.Services.QueryReader;
using SqlServerChallenges.Core.Services.QueryResultComparer;
using SqlServerChallenges.Core.Services.SampleOutput;

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
            => new SqlConnection(configuration.GetConnectionString("AdventureWorksConnection"))
        );

        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            options.AddOpenBehavior(typeof(CacheableQueryBehavior<,>));
            options.AddOpenBehavior(typeof(AnonymousRequestBehavior<,>));
        });

        services.AddMemoryCache();

        services.AddSingleton<MsSqlQuerySyntaxChecker>();

        services.AddScoped<IQueryExecutor, MsSqlQueryExecutor>();
        services.AddScoped<QueryExecutorDispatcher>();

        services.AddScoped<IQuerySyntaxChecker, MsSqlQuerySyntaxChecker>();
        services.AddScoped<SyntaxCheckerDispatcher>();

        services.AddScoped<IQueryReader, MsSqlQueryReader>();
        services.AddScoped<QueryReaderDispatcher>();

        services.AddSingleton<QueryResultComparer>();

        services.AddScoped<ISampleOutputProvider, SampleOutputProvider>();

        return services;
    }
}