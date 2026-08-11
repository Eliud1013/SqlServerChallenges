using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeInfo;
using SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeState;

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
        
        Info = infoResult.Value;

        return Page();
    }
}
