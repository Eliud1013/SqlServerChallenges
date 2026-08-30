namespace SqlServerChallenges.Core.Services.QueryExecutor;
public record QueryError(QueryErrorType Type, string Message);