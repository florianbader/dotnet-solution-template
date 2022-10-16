using System.Threading;
using System.Threading.Tasks;
using Application.Database;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Todo;

public class AddTodo
{
    public class Handler : IRequestHandler<Command, Todo>
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<AddTodo> _logger;
        private readonly IMapper _mapper;

        public Handler(ApplicationDbContext applicationDbContext, IMapper mapper, ILogger<AddTodo> logger)
            => (_applicationDbContext, _mapper, _logger) = (applicationDbContext, mapper, logger);

        public async Task<Todo> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Adding todo {@Request}", request);

            var todo = _mapper.Map<Todo>(request);
            var entityEntry = _applicationDbContext.Todo.Add(todo);

            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            _logger.LogTrace("Added todo with id {Id}", entityEntry.Entity.Id);

            return entityEntry.Entity;
        }
    }

    public record Command(string Name) : IRequest<Todo>;

    public class MappingProfile : Profile
    {
        public MappingProfile() =>
            CreateMap<Command, Todo>();
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(entity => entity.Name).NotEmpty().MaximumLength(256);
    }
}
