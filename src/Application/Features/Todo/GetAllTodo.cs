using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Todo;

public class GetAllTodo
{
    public class Handler : IRequestHandler<Query, IEnumerable<Todo>>
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<GetAllTodo> _logger;

        public Handler(ApplicationDbContext applicationDbContext, ILogger<GetAllTodo> logger)
            => (_applicationDbContext, _logger) = (applicationDbContext, logger);

        public async Task<IEnumerable<Todo>> Handle(Query request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Getting todos");

            var entities = await _applicationDbContext.Todo
                .ToArrayAsync(cancellationToken);
            return entities;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2094:Classes should not be empty", Justification = "Query object without parameters")]
    public record Query() : IRequest<IEnumerable<Todo>>;
}
