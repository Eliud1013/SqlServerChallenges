using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Services.QueryExecutor;

namespace SqlServerChallenges.Core.Features.Submissions.RunUserSql;

public sealed record RunUserSqlCommand(
    Guid ChallengeId,
    string UserQuery,
    DatabaseProvider Provider) : ICommand<RunResult>;