namespace SqlServerChallenges.Core.Services;

public sealed record SqlSyntaxError(int Line, int Column, string Message);

public sealed record SqlSyntaxResult(List<SqlSyntaxError> Errors)
{
    public bool IsValid => !Errors.Any();
    public bool IsInvalid => !IsValid;
};