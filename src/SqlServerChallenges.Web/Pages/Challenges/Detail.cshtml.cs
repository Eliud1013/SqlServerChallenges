using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data.Entities.ChallengeVote;
using SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeInfo;
using SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeState;
using SqlServerChallenges.Core.Features.Challenges.VoteChallenge;
using SqlServerChallenges.Web.Pages.Shared;

namespace SqlServerChallenges.Web.Pages.Challenges;

public class Detail : PageModel
{
    public ChallengeInfo Info { get; private set; } = null!;
    public ChallengeState State { get; private set; } = null!;
    public Guid ChallengeId { get; private set; }
    private readonly ISender _sender;
    public Detail(ISender sender)
    {
        _sender = sender;
    }
    
    public async Task<IActionResult> OnGet(Guid challengeId, string slug)
    {
        var infoResult = await _sender.Send(new GetChallengeInfoQuery(challengeId, slug));
        if (infoResult.Failed)
        {
            //IDK
        }
        
        var stateResult = await _sender.Send(new GetChallengeStateQuery(challengeId));
        if (stateResult.Failed)
        {
            //IDK
        }
        
        Info = infoResult.Value;
        State = stateResult.Value;
        ChallengeId = challengeId;

        return Page();
    }

    public async Task OnPostVote(Guid challengeId, VoteType type)
    {
        var result = await _sender.Send(new VoteChallengeCommand(challengeId, type));

        if (result.Failed)
        {
            // IDK
        }
    }
}
