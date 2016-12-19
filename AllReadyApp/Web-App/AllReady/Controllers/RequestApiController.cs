using System.Threading.Tasks;
using AllReady.Attributes;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AllReady.Features.Requests;
using AllReady.Hangfire.Jobs;
using AllReady.ViewModels.Requests;
using Hangfire;

namespace AllReady.Controllers
{
    //TODO mgmccarthy: use this route when token generation and TokenProtectedResource are sorted out
    //[Route("api/request")]
    [Route("api/requestapi")]
    [Produces("application/json")]
    public class RequestApiController : Controller
    {
        private readonly IMediator mediator;
        private readonly IBackgroundJobClient backgroundjobClient;

        public RequestApiController(IMediator mediator, IBackgroundJobClient backgroundjobClient)
        {
            this.backgroundjobClient = backgroundjobClient;
            this.mediator = mediator;
        }

        [HttpPost]
        [ExternalEndpoint]
        public async Task<IActionResult> Post([FromBody]RequestApiViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //we can only accept the requests with the status of "new" from getasmokealarm
            if (viewModel.Status != "new")
            {
                return BadRequest();
            }

            //if we get here, the incoming request is already in our database with a matching ProviderId ("serial" field for getasmokealarm) and the request was sent with a status of "new"
            if (await mediator.SendAsync(new RequestExistsByProviderIdQuery { ProviderRequestId = viewModel.ProviderRequestId }))
            {
                return BadRequest();
            }

            //this returns control to the caller immediately so the client is not left locked while we figure out if we can service the request
            backgroundjobClient.Enqueue<IProcessApiRequests>(x => x.Process(viewModel));

            //https://httpstatuses.com/202
            return StatusCode(202);
        }
    }
}