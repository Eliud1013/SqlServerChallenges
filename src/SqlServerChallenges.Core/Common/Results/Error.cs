using System.Diagnostics.CodeAnalysis;

namespace SqlServerChallenges.Core.Common.Results;

public sealed record Error
{
    public required ErrorType Type { get; init; }
    public required string Code { get; init; }
    public required string Message { get; init; }

    [SetsRequiredMembers]
    public Error(ErrorType type, string code, string message)
    {
        if (type != ErrorType.None && (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(message)))
            throw new InvalidOperationException("A non-None error must have a non-empty code and message.");

        Type = type;
        Code = code;
        Message = message;
    }

    public static Error None => new(ErrorType.None, string.Empty, string.Empty);
}