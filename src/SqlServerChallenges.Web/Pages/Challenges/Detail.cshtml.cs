using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Data.Entities.ChallengeVote;
using SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeInfo;
using SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeState;
using SqlServerChallenges.Core.Features.Challenges.VoteChallenge;
using SqlServerChallenges.Core.Features.Submissions.RunUserSql;

namespace SqlServerChallenges.Web.Pages.Challenges;

public class Detail : PageModel
{
    public ChallengeInfo Info { get; private set; } = null!;
    public ChallengeState State { get; private set; } = null!;
    private readonly ISender _sender;

    public Detail(ISender sender)
    {
        _sender = sender;
    }

    public async Task<IActionResult> OnGet(string slug)
    {
        var infoResult = await _sender.Send(new GetChallengeInfoQuery(slug));
        if (infoResult.Failed)
        {
            //IDK
        }

        var stateResult = await _sender.Send(new GetChallengeStateQuery(slug));
        if (stateResult.Failed)
        {
            //IDK
        }

        Info = infoResult.Value;
        State = stateResult.Value;

        return Page();
    }

    public async Task<IActionResult> OnPostRunCode(Guid challengeId, string sql)
    {
        var result = await _sender.Send(new RunUserSqlCommand(challengeId, sql, DatabaseProvider.SqlServer));

        if (result.Failed)
        {
            return BadRequest("Failed to execute query");
        }

        return Partial("RunResult", result.Value);
    }

    public async Task<IActionResult> OnPostVote(Guid ChallengeId, VoteType type)
    {
        if (ChallengeId == Guid.Empty)
        {
            //IDK
            return BadRequest();
        }

        var result = await _sender.Send(new VoteChallengeCommand(ChallengeId, type));

        if (result.Failed)
        {
            // IDK
            return BadRequest();
        }

        return new NoContentResult();
    }
}