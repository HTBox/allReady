using AllReady.Features.Campaigns;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AllReady.Features.Home;
using AllReady.ViewModels.Home;
using AllReady.ViewModels.Requests;
using System;
using AllReady.Features.Requests;

namespace AllReady.Controllers
{
    public class RequestStatusController : Controller
    {
        private readonly IMediator mediator;

        public RequestStatusController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("~/[controller]/{id}")]
        public async Task<IActionResult> Index(Guid id)
        {
            
            var model = await GetRequestStatus(id);

            if (model != null)
            {
                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        private async Task<RequestStatusViewModel> GetRequestStatus(Guid reqId)
        {
            return await mediator.SendAsync(new RequestStatusQuery { RequestId= reqId });
        }

    }
}