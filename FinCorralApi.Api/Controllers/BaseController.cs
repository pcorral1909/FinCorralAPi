using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinCorralApi.Api.Controllers
{
    
        [ApiController]
        [Route("api/[controller]")]
        public abstract class BaseController : ControllerBase
        {
            protected readonly IMediator Mediator;

            protected BaseController(IMediator mediator)
            {
                Mediator = mediator;
            }
        }
    

       
}
