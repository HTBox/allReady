using System;
using System.Threading.Tasks;
using AllReady.Attributes;
using Microsoft.AspNetCore.Mvc;
using AllReady.Models;
using MediatR;
using AllReady.Features.Requests;
using AllReady.ViewModels.Requests;

namespace AllReady.Controllers
{
    [Route("api/request")]
    [Produces("application/json")]
    public class RequestApiController : Controller
    {
        private readonly IMediator _mediator;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public RequestApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ExternalEndpoint]
        public async Task<IActionResult> Post([FromBody]RequestViewModel viewModel)
        {
            //validate before sending command
            if (viewModel.ProviderId == null)
            {
                return MapError(new AddRequestError { ProviderId = "", Reason = "No ProviderId" });
            }

            Guid requestId;
            if (!Guid.TryParse(viewModel.RequestId, out requestId))
            {
                return MapError(new AddRequestError { ProviderId = viewModel.ProviderId, Reason = "RequestId must be convertable to a Guid." });
            }

            if (!EnumCanBeMapped(viewModel.Status))
            {
                return MapError(new AddRequestError { ProviderId = viewModel.ProviderId, Reason = "enum string provided cannot be mapped to Request enum type." });
            }

            var result = await _mediator.SendAsync(new AddRequestCommand { RequestViewModel = viewModel });

            return Created(string.Empty, result);
        }

        private static bool EnumCanBeMapped(string stringStatus)
        {
            RequestStatus enumStatus;
            if (Enum.TryParse(stringStatus, out enumStatus))
            {
                return false;
            }

            return true;
        }

        private IActionResult MapError(AddRequestError error)
        {
            //I don't see anywhere this is set, so why do we have code for it?
            //if (error.IsInternal)
            //{
            //    return StatusCode(500, error);
            //}
            return BadRequest(error);
        }
    }
}