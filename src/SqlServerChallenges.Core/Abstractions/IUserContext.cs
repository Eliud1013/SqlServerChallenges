namespace SqlServerChallenges.Core.Authentication;

public interface IUserContext
{
    public bool IsAuthenticated { get; }
    public string? UserId { get; }
}