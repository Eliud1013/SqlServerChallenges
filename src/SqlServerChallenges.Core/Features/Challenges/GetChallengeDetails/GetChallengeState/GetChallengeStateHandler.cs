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
        var challengeState = await _dbContext.Challenges
            .Include(c => c.Votes.Where(vote => vote.Type == VoteType.UpVote))
            .Where(c => c.Id == request.ChallengeId)
            .Select(c => new ChallengeState(
                CommentCount: 0,
                UpVotes: c.Votes.Count,
                IsUpVoted: c.Votes.Any(vote =>
                    vote.UserId == _userContext.UserId
                    && vote.ChallengeId == request.ChallengeId),
                IsSolved: true)) // TODO: Determine whether the user has already solved the challenge 
            .FirstOrDefaultAsync(cancellationToken);

        if (challengeState is null)
            return ChallengesErrors.NotFound;

        return challengeState;
    }
}