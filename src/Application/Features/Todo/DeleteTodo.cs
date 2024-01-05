using Application.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Todo;

public class DeleteTodo
{
    public class Handler : IRequestHandler<Query>
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<DeleteTodo> _logger;

        public Handler(ApplicationDbContext applicationDbContext, ILogger<DeleteTodo> logger)
            => (_applicationDbContext, _logger) = (applicationDbContext, logger);

        public async Task Handle(Query request, CancellationToken cancellationToken)
        {
            await _applicationDbContext.Set<Todo>()
                .Where(todo => todo.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            _logger.LogTrace("Deleted todo with id {Id}", request.Id);
        }
    }

    public record Query(string Id) : IRequest;
}
