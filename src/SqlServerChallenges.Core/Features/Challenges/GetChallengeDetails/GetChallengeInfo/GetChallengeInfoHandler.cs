using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeInfo;

public class GetChallengeInfoHandler : IQueryHandler<GetChallengeInfoQuery,ChallengeInfo>
{
    private readonly ApplicationDbContext _dbContext;

    public GetChallengeInfoHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Result<ChallengeInfo>> Handle(GetChallengeInfoQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}