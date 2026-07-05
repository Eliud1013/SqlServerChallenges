using MediatR;
using SqlServerChallenges.Core.Common.Results;

namespace SqlServerChallenges.Core.Common.CQRS
{
    public interface ICommand : IRequest<Result>, IBaseCommand;
    
    public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;
    
    public interface IBaseCommand;
}