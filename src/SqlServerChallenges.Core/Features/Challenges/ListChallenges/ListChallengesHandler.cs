using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Data.Entities;

namespace SqlServerChallenges.Core.Features.Challenges.ListChallenges;

public class ListChallengesHandler : IQueryHandler<ListChallengesQuery, IReadOnlyList<ChallengeEntry>>
{
    private readonly ApplicationDbContext _dbContext;

    public ListChallengesHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<ChallengeEntry>>> Handle(ListChallengesQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Challenge> query = _dbContext.Challenges;

        if (request.Difficulty is not null)
            query = query.Where(c => c.Difficulty == request.Difficulty);

        if (!string.IsNullOrEmpty(request.CategoryName))
            query = query.Where(c => c.Category != null && c.Category.Name == request.CategoryName);

        if (!string.IsNullOrEmpty(request.Title))
            query = query.Where(c => c.Title.Contains(request.Title));

        int acceptance = RandomNumberGenerator.GetInt32(0, 100);
        
        return await query.Select(challenge => new ChallengeEntry(
            Id: challenge.Id,
            Title: challenge.Title,
            Description: challenge.TaskDescription,
            Category: challenge.Category.Name,
            Difficulty: challenge.Difficulty,
            Acceptance: acceptance,
            Solved: true)
        ).ToListAsync(cancellationToken);
    }
}