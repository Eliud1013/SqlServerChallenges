using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Data.Entities.Challenges;
using SqlServerChallenges.Core.Features.Challenges.ListChallenges;

namespace SqlServerChallenges.Web.Pages.Challenges.ListChallenges;

public class Index : PageModel
{
    private readonly ISender Sender;
    private readonly ApplicationDbContext _dbContext;

    public Index(ISender sender, ApplicationDbContext dbContext)
    {
        Sender = sender;
        _dbContext = dbContext;
    }

    public IReadOnlyList<ChallengeEntry> Challenges { get; private set; } = [];
    public IReadOnlyList<string> Categories { get; private set; } = [];

    public int TotalChallenges => Challenges.Count;
    public int EasyCount => Challenges.Count(challenge => challenge.Difficulty == ChallengeDifficulty.Easy);
    public int MediumCount => Challenges.Count(challenge => challenge.Difficulty == ChallengeDifficulty.Medium);
    public int HardCount => Challenges.Count(challenge => challenge.Difficulty == ChallengeDifficulty.Hard);

    public async Task OnGet()
    {
        /*var request = new ListChallengesQuery(null, null, null);
        var result = await Sender.Send(request);

        Challenges = result.Value;
        Categories = await _dbContext.Categories.OrderBy(c => c.Name).Select(c => c.Name).ToListAsync();*/
    }

    public async Task<IActionResult> OnGetUpdateList(string? Title, string? CategoryName,
        ChallengeDifficulty? Difficulty)
    {
        var request = new ListChallengesQuery(Title, CategoryName, Difficulty);
        var result = await Sender.Send(request);
        
        return Partial("_ChallengeRows", result.Value);
    }
}