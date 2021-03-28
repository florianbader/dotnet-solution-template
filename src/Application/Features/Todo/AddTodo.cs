using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.Todo
{
    public static class AddTodo
    {
        public class Handler : IRequestHandler<Query, Todo>
        {
            public Task<Todo> Handle(Query request, CancellationToken cancellationToken) => Task.FromResult(new Todo("id", "name", false));
        }

        public record Query : IRequest<Todo>;
    }
}
