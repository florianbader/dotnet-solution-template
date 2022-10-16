using System.Threading;
using System.Threading.Tasks;
using Application.Database;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Todo;

public class GetTodo
{
    public class Handler : IRequestHandler<Query, Todo>
    {
        private readonly ApplicationDbContext _applicationDbContext;

        private readonly ILogger<GetTodo> _logger;

        public Handler(ApplicationDbContext applicationDbContext, ILogger<GetTodo> logger)
            => (_applicationDbContext, _logger) = (applicationDbContext, logger);

        public async Task<Todo> Handle(Query request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Getting todo with id {Id}", request.Id);

            var entity = await _applicationDbContext.Todo
                .GetOrThrowAsync(entity => entity.Id == request.Id, cancellationToken);
            return entity;
        }
    }

    public record Query(string Id) : IRequest<Todo>;
}
