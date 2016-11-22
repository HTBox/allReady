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

        public RequestApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ExternalEndpoint]
        public async Task<IActionResult> Post([FromBody]RequestViewModel viewModel)
        {
            //TODO: identify required fields on Request

            //validate before sending command
            if (viewModel.ProviderId == null)
            {
                return MapError(new AddRequestError { ProviderId = "", Reason = "No ProviderId" });
            }

            if (string.IsNullOrEmpty(viewModel.Status))
            {
                //return error or set error in message that will delivered to their API
            }

            //we only accept the status of "new" from RC integration, the rest we ignore
            if (!string.IsNullOrEmpty(viewModel.Status))
            {
                if (viewModel.Status != "new")
                {
                    //return error or set error in the message that will delivered to their API OR drop it on the floor
                }
            }

            //TODO: look up the Request by ProviderId (for red cross, that's "serial") to make sure it does not already exist in the database. If it does, we don't want to process the message b/c it's not a "new" status 

            var result = await _mediator.SendAsync(new AddApiRequestCommand { RequestViewModel = viewModel });

            //TODO: return a 202 http status code with a true/false attached to the response saying we either accepted the request (true) or we didn't accept the request (false). 
            //In order to accept a request (we currently are "serviing" that region), we'd have to add some type of region code to Organization in order to do a lookup.
            //BUT, Tony Surma says here: https://github.com/redcross/smoke-alarm-portal/issues/196#issuecomment-238036967 
            //that we should NOT add region code to Organzation b/c it's red cross specific... we should instead issue a token per supported region and use that to determine whether or not we can service the request
            //https://httpstatuses.com/202
            return Created(string.Empty, result);
        }

        private IActionResult MapError(AddRequestError error)
        {
            return BadRequest(error);
        }
    }
}