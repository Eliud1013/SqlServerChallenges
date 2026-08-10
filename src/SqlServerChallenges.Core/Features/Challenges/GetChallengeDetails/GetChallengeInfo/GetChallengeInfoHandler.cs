using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Services.SampleOutput;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeInfo;

public class GetChallengeInfoHandler : IQueryHandler<GetChallengeInfoQuery, ChallengeInfo>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ISampleOutputProvider _sampleOutputProvider;

    public GetChallengeInfoHandler(ApplicationDbContext dbContext, ISampleOutputProvider sampleOutputProvider)
    {
        _dbContext = dbContext;
        _sampleOutputProvider = sampleOutputProvider;
    }

    public async Task<Result<ChallengeInfo>> Handle(GetChallengeInfoQuery request, CancellationToken cancellationToken)
    {
        var challenge = await _dbContext.Challenges
            .Include(c => c.Category)
            .FirstOrDefaultAsync(c => c.Slug == request.Slug, cancellationToken);

        if (challenge is null)
            return ChallengesErrors.NotFound;

        var sampleOutput = await _sampleOutputProvider.GetForChallengeAsync(challenge.Id, rowLimit: 50, cancellationToken);

        if (sampleOutput.Failed)
            return ChallengesErrors.UnknownError;

        return new ChallengeInfo(
            challenge.Title,
            challenge.TaskDescription,
            challenge.Difficulty,
            challenge.Category.Name,
            sampleOutput.Value);
    }
}