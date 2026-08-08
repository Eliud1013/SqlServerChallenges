using MediatR;
using Microsoft.Extensions.Caching.Memory;
using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Common.Results;

namespace SqlServerChallenges.Core.Common.Behaviours;

public class CacheableQueryBehavior<TQuery, TResponse> : IPipelineBehavior<TQuery, Result<TResponse>>
    where TQuery : ICacheableQuery<TResponse>
{
    private readonly IMemoryCache _cache;

    public CacheableQueryBehavior(IMemoryCache cache)
    {
        _cache = cache;
    }
    
    public async Task<Result<TResponse>> Handle(
        TQuery request,
        RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(request.CacheKey, out object? cached) && cached is Result<TResponse> hit)
            return hit;

        var result = await next(cancellationToken);

        if (result is Result { Succeeded: true })
            _cache.Set(request.CacheKey, result, new MemoryCacheEntryOptions
            {
                SlidingExpiration = request.SlidingExpiration,
                AbsoluteExpiration = request.AbsoluteExpiration
            });

        return result;
    }
}
