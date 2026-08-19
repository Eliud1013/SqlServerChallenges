using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Data.Entities.ChallengeVote;

namespace SqlServerChallenges.Core.Features.Challenges.VoteChallenge;

public record VoteChallengeCommand(Guid ChallengeId, VoteType Type) : ICommand;