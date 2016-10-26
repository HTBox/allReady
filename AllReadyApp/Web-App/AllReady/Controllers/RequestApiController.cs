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
    [Route("api/requestapi")]
    [Produces("application/json")]
    public class RequestApiController : Controller
    {
        private readonly IMediator _mediator;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public RequestApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //TODO mgmccarthy: why do we need a strongly typed error to return to the caller?  Is there a requirement for this? I didn't touch the AddRequestError b/c it was there in the original PR #1111
        [HttpPost]
        [ExternalEndpoint]
        public async Task<IActionResult> Post([FromBody]RequestViewModel viewModel)
        {
            //validate before sending command
            if (viewModel.ProviderId == null)
            {
                return MapError(new AddRequestError { ProviderId = "", Reason = "No ProviderId" });
            }

            if (!string.IsNullOrEmpty(viewModel.RequestId))
            {
                Guid requestId;
                if (!Guid.TryParse(viewModel.RequestId, out requestId))
                {
                    return MapError(new AddRequestError { ProviderId = viewModel.ProviderId, Reason = "RequestId must be convertable to a Guid." });
                }
            }

            //TODO mgmccarthy: making the assumption here that we'll receive an empty Status for a new Request. There might be a specific status we get from the incoming request
            if (!string.IsNullOrEmpty(viewModel.Status))
            {
                if (!StatusCanBeMappedToRequestStatusEnum(viewModel.Status))
                {
                    return MapError(new AddRequestError { ProviderId = viewModel.ProviderId, Reason = "enum string provided cannot be mapped to Request enum type." });
                }
            }
            
            var result = await _mediator.SendAsync(new AddRequestCommand { RequestViewModel = viewModel });

            //TODO mgmccarthy: I'm not too sure why we have to return the entire result. I'd rather just return a 200 OK Http status or return the RequestId so the requestor can correlate a request back to our system when sending us updates
            return Created(string.Empty, result);
        }

        private static bool StatusCanBeMappedToRequestStatusEnum(string stringStatus)
        {
            RequestStatus enumStatus;
            if (Enum.TryParse(stringStatus, out enumStatus))
            {
                return true;
            }

            return false;
        }

        private IActionResult MapError(AddRequestError error)
        {
            //TODO mgmccarthy: I don't see anywhere this is set, so I commented it out
            //if (error.IsInternal)
            //{
            //    return StatusCode(500, error);
            //}
            return BadRequest(error);
        }
    }
}