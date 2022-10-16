using System.Threading;
using System.Threading.Tasks;
using Application.Database;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Todo;

public class DeleteTodo
{
    public class Handler : AsyncRequestHandler<Query>
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<DeleteTodo> _logger;

        public Handler(ApplicationDbContext applicationDbContext, ILogger<DeleteTodo> logger)
            => (_applicationDbContext, _logger) = (applicationDbContext, logger);

        protected override async Task Handle(Query request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.Todo.GetOrThrowAsync(entity => entity.Id == request.Id, cancellationToken);

            _applicationDbContext.Todo.Remove(entity);

            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            _logger.LogTrace("Deleted todo with id {Id}", request.Id);
        }
    }

    public record Query(string Id) : IRequest;
}
