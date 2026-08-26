namespace SqlServerChallenges.Core.Services.QueryExecutor;

public enum QueryErrorType
{
    QueryTimeout,
    PermissionDenied,
    InvalidQuery,
    Unknown
}