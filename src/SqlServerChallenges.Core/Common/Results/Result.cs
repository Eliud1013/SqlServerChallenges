namespace SqlServerChallenges.Core.Common.Results;

public class Result
{
    public bool Succeeded { get; }
    public Error Error { get; }
    public bool Failed => !Succeeded;

    protected Result(bool succeeded, Error error)
    {
        if (succeeded != (error == Error.None))
        {
            throw new InvalidOperationException(
                "A successful result must have Error.None, and a failed result must have a non-None error.");
        }

        Succeeded = succeeded;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);

    public static Result Fail(Error error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    public static Result<TValue> Fail<TValue>(Error error) =>
        new(default, false, error);

    public static Result<TValue> Create<TValue>(TValue? value, Error error)
    {
        if (value is null == (error == Error.None))
        {
            throw new InvalidOperationException(
                "Provide either a value for a successful result or a non-None error for a failed result.");
        }

        return value is not null
            ? Success(value)
            : Fail<TValue>(error);
    }

    public static implicit operator Result(Error error) => Fail(error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool succeeded, Error error)
        : base(succeeded, error)
    {
        if (succeeded && value is null)
        {
            throw new InvalidOperationException(
                "A successful result must contain a non-null value.");
        }

        _value = value;
    }

    public TValue Value =>
        Succeeded
            ? _value!
            : throw new InvalidOperationException(
                "The value of a failed result cannot be accessed.");

    public static implicit operator Result<TValue>(TValue? value) =>
        Create(value, Error.None);

    public static implicit operator Result<TValue>(Error error) =>
        Fail<TValue>(error);
}