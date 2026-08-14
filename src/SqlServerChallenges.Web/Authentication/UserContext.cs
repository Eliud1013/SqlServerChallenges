using SqlServerChallenges.Core.Authentication;

namespace SqlServerChallenges.Web.Authentication;

public class UserContext : IUserContext
{
    public bool IsAuthenticated { get; } = true;
    public string? UserId { get; } = "0";
}