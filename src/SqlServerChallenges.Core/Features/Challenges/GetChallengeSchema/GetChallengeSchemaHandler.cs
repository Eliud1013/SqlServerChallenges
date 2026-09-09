using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Services.SchemaReader;
using SqlServerChallenges.Core.Services.TableReferenceExtractor;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeSchema;

public class GetChallengeSchemaHandler : IQueryHandler<GetChallengeSchemaQuery, IReadOnlyDictionary<string, IReadOnlyList<ColumnInfo>>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly TableReferencesExtractorDispatcher _referencesExtractorDispatcher;
    private readonly InformationSchemaReader _schemaReader;

    public GetChallengeSchemaHandler(
        TableReferencesExtractorDispatcher referencesExtractorDispatcher,
        ApplicationDbContext dbContext, InformationSchemaReader schemaReader)
    {
        _referencesExtractorDispatcher = referencesExtractorDispatcher;
        _dbContext = dbContext;
        _schemaReader = schemaReader;
    }

    public async Task<Result<IReadOnlyDictionary<string, IReadOnlyList<ColumnInfo>>>> Handle(
        GetChallengeSchemaQuery request,
        CancellationToken cancellationToken)
    {
        var challengeExists = await _dbContext.Challenges
            .AnyAsync(c => c.Id == request.ChallengeId, cancellationToken);

        if (!challengeExists)
            return ChallengesErrors.NotFound;

        var solution = await _dbContext.Solutions
            .FirstOrDefaultAsync(c => c.ChallengeId == request.ChallengeId && c.DatabaseProvider == request.Provider,
                cancellationToken);
        
        if (solution is null)
            return ChallengesErrors.SolutionNotFound;

        var tables = _referencesExtractorDispatcher.Extract(solution.SolutionSql, request.Provider);
        var schema = await _schemaReader.ReadColumnsAsync(tables, request.Provider, cancellationToken);

        return Result.Success(schema);
    }
}