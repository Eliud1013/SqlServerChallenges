using System.Diagnostics.CodeAnalysis;

namespace SqlServerChallenges.Core.Common.Results;

public record Error
{
    public string Code { get; init; }
    public string Message { get; init; }

    public Error(string code, string message)
    {
        if (string.IsNullOrEmpty(code) ^ string.IsNullOrEmpty(message))
            throw new InvalidOperationException("Code and Message must both be either provided or empty.");

        Code = code;
        Message = message;
    }

    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The operation resulted in a null value.");
}

public sealed record Error<TValue> : Error
{
    private readonly TValue _value;

    public Error(
        string code,
        string message,
        TValue value) : base(code, message)
    {
        _value = value;
    }

    public TValue Value => !string.IsNullOrEmpty(Code)
        ? _value
        : throw new InvalidOperationException("Value on a non-error cannot be accessed.");
}