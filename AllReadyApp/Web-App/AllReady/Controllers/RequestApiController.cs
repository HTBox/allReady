using System.Threading.Tasks;
using AllReady.Attributes;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Post([FromBody]RequestApiViewModel viewModel)
        {
            //TODO: I'm making a guess that field validations will return a BadRequest result instead of a 202 using ModelBinding. Testing it with PostMan, the model bineder worked
            //Anything that could potentially take longer then simple field validation (aka, region validation) will be moved further down the pipelines to be reported back to getasmokealarm's API
            //waiting to hear back from the getasmokealarm folks if they take specific actions from the ack from our endpoint.

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            //we only accept the status of "new" from RC integration, the rest we ignore
            if (viewModel.Status != "new")
            {
                return BadRequest();
            }

            //if we get here, the incoming request has mistakenly been labeled with the "new" status code
            if (_mediator.Send(new RequestExistsByProviderIdQuery { RequestProviderId = viewModel.ProviderRequestId }))
            {
                return BadRequest();
            }

            //TODO: region specific verification (this COULD be moved further down the pipeline to have the request status reported back to getasmokealarm via their API)

            //TODO: waiting to hear back from getasmokealarm what data they would expect back on the ack, if they only require the 202 back and we can invoke their API downstream from this to report back whether or not we're going to accept this request.
            await _mediator.SendAsync(new AddApiRequestCommand { ViewModel = viewModel });

            //https://httpstatuses.com/202
            return StatusCode(202);
        }
    }
}