using MediatR;
using SqlServerChallenges.Core.Common.Results;

namespace SqlServerChallenges.Core.Common.CQRS.Query;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;