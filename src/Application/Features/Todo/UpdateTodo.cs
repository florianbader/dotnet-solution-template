using System.Threading;
using System.Threading.Tasks;
using Application.Database;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Todo
{
    public class UpdateTodo
    {
        public class Handler : IRequestHandler<Command, Todo>
        {
            private readonly ApplicationDbContext _applicationDbContext;
            private readonly ILogger<UpdateTodo> _logger;
            private readonly IMapper _mapper;

            public Handler(ApplicationDbContext applicationDbContext, IMapper mapper, ILogger<UpdateTodo> logger)
                => (_applicationDbContext, _mapper, _logger) = (applicationDbContext, mapper, logger);

            public async Task<Todo> Handle(Command request, CancellationToken cancellationToken)
            {
                _logger.LogTrace("Updating todo {@Request}", request);

                var entity = await _applicationDbContext.Todo.GetOrThrowAsync(entity => entity.Id == request.Id, cancellationToken);
                _mapper.Map(request, entity);

                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                _logger.LogTrace("Updated todo with id {Id}", entity.Id);

                return entity;
            }
        }

        public record Command(string Id, string Name, bool IsDone) : IRequest<Todo>;

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Command, Todo>();
        }
    }
}
