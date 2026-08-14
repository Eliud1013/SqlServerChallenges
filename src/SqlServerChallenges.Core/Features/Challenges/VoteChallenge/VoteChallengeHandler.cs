using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Authentication;
using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Data.Entities.ChallengeVote;
using SqlServerChallenges.Core.Features.Challenges;

namespace SqlServerChallenges.Core.Features.Challenges.VoteChallenge;

public class VoteChallengeHandler : ICommandHandler<VoteChallengeCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _clock;

    public VoteChallengeHandler(
        IUserContext userContext,
        ApplicationDbContext dbContext, IDateTimeProvider clock)
    {
        _userContext = userContext;
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task<Result> Handle(VoteChallengeCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId!;
        var challengeId = request.ChallengeId;

        var challengeExists = await _dbContext.Challenges
            .AnyAsync(c => c.Id == challengeId, cancellationToken);

        if (!challengeExists)
            return ChallengesErrors.NotFound;

        var now = _clock.UtcNow;
        var currentVote = await _dbContext.Votes
            .FirstOrDefaultAsync(v => v.ChallengeId == challengeId && v.UserId == userId,
                cancellationToken);

        if (currentVote is null)
        {
            _dbContext.Votes.Add(new ChallengeVote
            {
                ChallengeId = challengeId,
                UserId = userId,
                Type = request.Type,
                VotedAtUtc = now
            });
        }
        else
        {
            if (currentVote.Type == request.Type)
            {
                _dbContext.Votes.Remove(currentVote);
            }
            else
            {
                currentVote.Type = request.Type;
                currentVote.VotedAtUtc = now;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
