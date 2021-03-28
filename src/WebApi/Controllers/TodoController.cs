using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.Todo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class TodoController : ApiControllerBase
    {
        public TodoController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public Task<IEnumerable<Todo>> GetAllAsync() => Mediator.Send(new GetAllTodo.Query());
    }
}
