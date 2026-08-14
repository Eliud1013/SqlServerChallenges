using MediatR;
using SqlServerChallenges.Core.Authentication;
using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Common.Results;

namespace SqlServerChallenges.Core.Common.Behaviours;

public class AuthenticatedRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUserContext _userContext;

    public AuthenticatedRequestBehavior(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IAuthenticatedRequest)
            return await next(cancellationToken);

        if (!_userContext.IsAuthenticated)
            return Unauthorized<TResponse>();

        return await next(cancellationToken);
    }

    private static TResult Unauthorized<TResult>()
    {
        if (typeof(TResult) == typeof(Result))
            return (TResult)(object)Result.Fail(Error.Unauthorized);

        var valueType = typeof(TResult).GetGenericArguments()[0];
        var fail = typeof(Result)
            .GetMethod(nameof(Result.Fail), new[] { typeof(Error) })!
            .MakeGenericMethod(valueType);

        return (TResult)fail.Invoke(null, new object[] { Error.Unauthorized })!;
    }
}
