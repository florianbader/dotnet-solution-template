using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.Todo
{
    public static class DeleteTodo
    {
        public class Handler : AsyncRequestHandler<Query>
        {
            protected override Task Handle(Query request, CancellationToken cancellationToken) => Task.CompletedTask;
        }

        public record Query(string Id) : IRequest;
    }
}
