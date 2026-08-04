using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeState;

public class GetChallengeStateHandler : IQueryHandler<GetChallengeStateQuery, ChallengeState>
{
    private readonly ApplicationDbContext _dbContext;

    public GetChallengeStateHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Result<ChallengeState>> Handle(GetChallengeStateQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
