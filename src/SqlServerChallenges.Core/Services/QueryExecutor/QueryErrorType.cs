namespace SqlServerChallenges.Core.Services.QueryExecutor;

public enum QueryErrorType
{
    QueryTimeout,
    InvalidColumn,
    PermissionDenied,
    InvalidQuery,
    Unknown
}