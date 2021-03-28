using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.Todo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/todo")]
    public class TodoController : ApiControllerBase
    {
        public TodoController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpPost]
        public Task<Todo> AddAsync(AddTodo.Command todo) => Mediator.Send(todo);

        [HttpGet]
        public Task<IEnumerable<Todo>> GetAllAsync() => Mediator.Send(new GetAllTodo.Query());

        [HttpGet("{id}")]
        public Task<Todo> GetAsync(string id) => Mediator.Send(new GetTodo.Query(id));

        [HttpPut("{id}")]
        public Task<Todo> UpdateAsync(string id, UpdateTodo.Command todo) => Mediator.Send(todo with { Id = id });
    }
}
