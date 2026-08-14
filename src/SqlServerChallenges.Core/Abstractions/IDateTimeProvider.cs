namespace SqlServerChallenges.Core.Authentication;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}