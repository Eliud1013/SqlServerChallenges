namespace SqlServerChallenges.Core.Services.QueryExecutor;

public enum QueryErrorType
{
    QueryTimeout,
    InvalidIdentifier,
    PermissionDenied,
    InvalidQuery,
    Unknown
}