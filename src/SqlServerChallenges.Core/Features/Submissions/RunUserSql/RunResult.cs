using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.QueryExecutor;

namespace SqlServerChallenges.Core.Features.Submissions.RunUserSql;

public sealed record RunResult
{
    public bool IsSuccess { get; }
    public IReadOnlyList<SqlSyntaxError>? SyntaxErrors { get; }
    public QueryErrorType? ErrorType { get; }
    public string? ErrorMessage { get; }

    public IReadOnlyList<string> UserColumns { get; }
    public IReadOnlyList<string> ExpectedColumns { get; }

    public int UserRowCount { get; }
    public int ExpectedRowCount { get; }

    public IReadOnlyList<IReadOnlyDictionary<string, object?>> UserRows { get; }
    public IReadOnlyList<IReadOnlyDictionary<string, object?>> ExpectedRows { get; }

    public bool ColumnsMatch { get; }
    public bool ColumnCountMatch { get; }
    public bool RowCountMatch { get; }

    private RunResult(QueryErrorType errorType, string errorMessage)
    {
        IsSuccess = false;
        ErrorType = errorType;
        ErrorMessage = errorMessage;
        UserColumns = [];
        ExpectedColumns = [];
        UserRows = [];
        ExpectedRows = [];
    }
    
    private RunResult(IReadOnlyList<SqlSyntaxError> syntaxErrors)
    {
        IsSuccess = false;
        SyntaxErrors = syntaxErrors;
        UserColumns = [];
        ExpectedColumns = [];
        UserRows = [];
        ExpectedRows = [];
    }

    private RunResult(OutputTable userResult, OutputTable expectedResult)
    {
        IsSuccess = true;
        UserColumns = userResult.Columns;
        ExpectedColumns = expectedResult.Columns;
        UserRowCount = userResult.Rows.Count;
        ExpectedRowCount = expectedResult.Rows.Count;
        UserRows = userResult.Rows;
        ExpectedRows = expectedResult.Rows;

        ColumnCountMatch = UserColumns.Count == ExpectedColumns.Count;
        ColumnsMatch = UserColumns.SequenceEqual(ExpectedColumns, StringComparer.OrdinalIgnoreCase);
        RowCountMatch = UserRowCount == ExpectedRowCount;
    }

    public static RunResult Error(QueryErrorType type, string message) => new(type, message);
    public static RunResult SyntaxError(IReadOnlyList<SqlSyntaxError> errors) => new (errors);
    public static RunResult FromResults(OutputTable userResult, OutputTable expectedResult) =>
        new(userResult, expectedResult);

    public bool IsRowMatch(int index)
    {
        if (index >= UserRows.Count || index >= ExpectedRows.Count)
            return false;

        return RowValuesEqual(UserRows[index], ExpectedRows[index]);
    }

    private static bool RowValuesEqual(
        IReadOnlyDictionary<string, object?> a,
        IReadOnlyDictionary<string, object?> b)
    {
        if (a.Count != b.Count) return false;

        foreach (var kvp in a)
        {
            if (!b.TryGetValue(kvp.Key, out var otherVal)) return false;

            var aStr = kvp.Value?.ToString() ?? "";
            var bStr = otherVal?.ToString() ?? "";

            if (!string.Equals(aStr, bStr, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }
}
