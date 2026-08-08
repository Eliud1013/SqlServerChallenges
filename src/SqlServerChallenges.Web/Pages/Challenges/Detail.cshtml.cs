using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SqlServerChallenges.Core.Data.Entities.Challenges;
using SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeInfo;
using SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeState;
using SqlServerChallenges.Core.Services.QueryExecutor;

namespace SqlServerChallenges.Web.Pages.Challenges;

public class Detail : PageModel
{
    public ChallengeInfo Info { get; private set; } = null!;
    public ChallengeState State { get; private set; } = null!;
    public string Slug { get; private set; } = null!;

    public IActionResult OnGet(string slug)
    {
        var rows = new List<string>();
        var cols = new List<Dictionary<string, object?>>();
        
        Slug = slug;
        Info = new ChallengeInfo(
            "SELECT All",
            "Write a query to return all columns from the Person.Person table.",
            ChallengeDifficulty.Easy,
            "Basic Queries",
            new OutputTable(rows,cols));

        State = new ChallengeState(CommentCount: 3, IsSolved: false);

        return Page();
    }
}
