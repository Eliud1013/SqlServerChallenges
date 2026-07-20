using System.Data;
using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Features.Submissions.RunUserQuery;

public sealed record RunUserSqlCommand(
    Guid ChallengeId,
    string UserQuery,
    DatabaseProvider provider) : ICommand<QueryExecutorResult>;