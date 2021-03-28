using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.Todo
{
    public static class GetAllTodo
    {
        public class Handler : IRequestHandler<Query, IEnumerable<Todo>>
        {
            public Task<IEnumerable<Todo>> Handle(Query request, CancellationToken cancellationToken)
                => Task.FromResult(new[] { new Todo("id", "name", false) }.AsEnumerable());
        }

        public record Query() : IRequest<IEnumerable<Todo>>;
    }
}
