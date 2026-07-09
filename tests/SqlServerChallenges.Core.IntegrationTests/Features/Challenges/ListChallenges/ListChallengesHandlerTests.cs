using SqlServerChallenges.Core.IntegrationTests.Common;
using Xunit;

namespace SqlServerChallenges.Core.IntegrationTests.Features.Challenges.ListChallenges;

public class ListChallengesHandlerTests :IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;
    

    public ListChallengesHandlerTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }
}