using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class TodoController : ApiControllerBase
    {
        public TodoController(IMediator mediator) : base(mediator) { }

        [HttpGet] public Task<Todo> GetAllAsync() => Mediator.SendAsync(new);
    }
}
