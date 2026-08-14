using SqlServerChallenges.Core.Authentication;

namespace SqlServerChallenges.Web.Common;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}