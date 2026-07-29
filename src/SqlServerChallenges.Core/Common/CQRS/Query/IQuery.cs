using MediatR;
using SqlServerChallenges.Core.Common.Results;

namespace SqlServerChallenges.Core.Common.CQRS.Query;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;

public interface ICacheableQuery<TResponse> : IQuery<TResponse>
{
    string CacheKey { get; }
    TimeSpan? SlidingExpiration { get; }
    DateTimeOffset? AbsoluteExpiration { get; }
}