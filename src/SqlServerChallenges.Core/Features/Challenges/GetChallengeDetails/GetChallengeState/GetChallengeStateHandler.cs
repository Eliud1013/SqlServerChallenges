using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Authentication;
using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Data.Entities;
using SqlServerChallenges.Core.Data.Entities.ChallengeVote;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeState;

public class GetChallengeStateHandler : IQueryHandler<GetChallengeStateQuery, ChallengeState>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public GetChallengeStateHandler(
        ApplicationDbContext dbContext,
        IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<ChallengeState>> Handle(GetChallengeStateQuery request,
        CancellationToken cancellationToken)
    {
        var challengeId = await _dbContext.Challenges
            .Where(c => c.Slug == request.Slug)
            .Select(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (challengeId == Guid.Empty)
            return ChallengesErrors.NotFound;
        
        var challengeState = await _dbContext.Challenges
            .Where(c => c.Slug == request.Slug)
            .Select(c => new ChallengeState(
                CommentCount: 0,
                UpVotes: c.Votes.Count(v => v.Type == VoteType.UpVote),
                IsUpVoted: c.Votes.Any(v => v.UserId == _userContext.UserId && v.Type == VoteType.UpVote),
                IsDownVoted: c.Votes.Any(v => v.UserId == _userContext.UserId && v.Type == VoteType.DownVote),
                IsSolved: true // TODO: Determine whether the user has already solved the challenge
            )).FirstOrDefaultAsync(cancellationToken);

        if (challengeState is null)
            return ChallengesErrors.NotFound;

        return challengeState;
    }
}