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

        public RequestApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ExternalEndpoint]
        public async Task<IActionResult> Post([FromBody]RequestViewModel viewModel)
        {
            if (viewModel.ProviderId == null)
            {
                return MapError(new AddRequestError { ProviderId = "", Reason = "No ProviderId" });
            }

            var request = ToModel(viewModel);

            await _mediator.SendAsync(new AddRequestCommand { Request = request });

            return Created(string.Empty, request);
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

        private static Request ToModel(RequestViewModel requestViewModel)
        {
            var request = new Request
            {
                ProviderId = requestViewModel.ProviderId,
                ProviderData = requestViewModel.ProviderData,
                Address = requestViewModel.Address,
                City = requestViewModel.City,
                DateAdded = DateTime.UtcNow,
                Email = requestViewModel.Email,
                Name = requestViewModel.Name,
                Phone = requestViewModel.Phone,
                State = requestViewModel.State,
                Zip = requestViewModel.Zip,
                Status = RequestStatus.Unassigned,
                Latitude = requestViewModel.Latitude,
                Longitude = requestViewModel.Longitude
            };

            RequestStatus status;
            if (Enum.TryParse(requestViewModel.Status, out status))
            {
                request.Status = status;
            }

            return request;
        }
    }
}