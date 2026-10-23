using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Authentication;
using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Data.Entities;

namespace SqlServerChallenges.Core.Features.Challenges.ListChallenges;

public class ListChallengesHandler : IQueryHandler<ListChallengesQuery, IReadOnlyList<ChallengeEntry>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public ListChallengesHandler(
        ApplicationDbContext dbContext,
        IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
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

        var solvedChallenges = await _dbContext.Submissions
            .Where(s => s.UserId == _userContext.UserId && s.Passed)
            .Select(s => s.ChallengeId)
            .ToHashSetAsync(cancellationToken);
        
        return await query
            .OrderBy(c => c.Number)
            .Select(challenge => new ChallengeEntry(
                Id: challenge.Id,
                Title: challenge.Title,
                Slug: challenge.Slug,
                Number: challenge.Number,
                Description: challenge.TaskDescription,
                Category: challenge.Category.Name,
                Difficulty: challenge.Difficulty,
                Acceptance: acceptance,
                Solved: solvedChallenges.Contains(challenge.Id))
            ).ToListAsync(cancellationToken);
    }
}