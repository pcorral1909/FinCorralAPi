using Microsoft.AspNetCore.Mvc;
using FinCorralApi.Api.Extensions;
using FinCorralApi.Application.Command.CreateSample;
using MediatR;

namespace FinCorralApi.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SampleController : BaseController
    {
        public SampleController(IMediator mediator) : base(mediator) { }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSampleCommand command)
        {
            var result = await Mediator.Send(command);
            return result.ToActionResult();
        }
    }

}
