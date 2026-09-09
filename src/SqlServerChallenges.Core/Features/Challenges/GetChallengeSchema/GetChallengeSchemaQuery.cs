using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Services.SchemaReader;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeSchema;

public record GetChallengeSchemaQuery(Guid ChallengeId, DatabaseProvider Provider)
    : IQuery<IReadOnlyDictionary<string, IReadOnlyList<ColumnInfo>>>;