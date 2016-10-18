using System;
using System.Threading.Tasks;
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RequestViewModel viewModel)
        {
            //validate before sending command
            if (viewModel.ProviderId == null)
            {
                return MapError(new AddRequestError { ProviderId = "", Reason = "No ProviderId" });
            }

            Guid requestId;
            if (!string.IsNullOrEmpty(viewModel.RequestId))
            {
                if (!Guid.TryParse(viewModel.RequestId, out requestId))
                {
                    return MapError(new AddRequestError { ProviderId = viewModel.ProviderId, Reason = "RequestId must be convertable to a Guid." });
                }
            }

            //TODO mgmccarthy: making the assumption here that we'll receive an empty Status for a new Request.
            if (!string.IsNullOrEmpty(viewModel.Status))
            {
                if (!EnumCanBeMapped(viewModel.Status))
                {
                    return MapError(new AddRequestError { ProviderId = viewModel.ProviderId, Reason = "enum string provided cannot be mapped to Request enum type." });
                }
            }
            
            var result = await _mediator.SendAsync(new AddRequestCommand { RequestViewModel = viewModel });

            //TODO mgmccarthy
            //when returning Result, the json coming back for the Status propert is the integeter value, not the enum name as a string, so this:
            //"Status": 3,
            //instead of this,
            //"Status": "Canceled"
            //is this what we want?
            return Created(string.Empty, result);
        }

        private static bool EnumCanBeMapped(string stringStatus)
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
            //I don't see anywhere this is set, so why do we have code for it?
            //if (error.IsInternal)
            //{
            //    return StatusCode(500, error);
            //}
            return BadRequest(error);
        }
    }
}