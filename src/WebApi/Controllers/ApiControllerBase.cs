using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        public ApiControllerBase(IMediator mediator) => Mediator = mediator;

        protected IMediator Mediator { get; }
    }
}
