namespace SqlServerChallenges.Core.Services.QueryRateLimiter;

public static class QueryCacheKeys
{
    public const string Run = "ratelimit:query:run";
    public const string Submit = "QueryExecutor";
}