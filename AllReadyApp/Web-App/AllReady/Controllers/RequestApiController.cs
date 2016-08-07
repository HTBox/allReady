using System;
using System.Threading.Tasks;
using AllReady.Attributes;
using Microsoft.AspNetCore.Mvc;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
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
        public async Task<IActionResult> Post([FromBody]RequestViewModel request)
        {
            var allReadyRequest = ToModel(request, _mediator);

            AddRequestError error = await _mediator.SendAsync(new AddRequestCommandAsync { Request = allReadyRequest });

            if (error != null)
            {
                return MapError(error);
            }
            return Created(string.Empty, allReadyRequest);
        }

        private IActionResult MapError(AddRequestError error)
        {
            if (error.IsInternal)
            {
                return StatusCode(500, error);
            }
            else
            {
                return BadRequest(error);
            }
        }

        public Request ToModel(RequestViewModel requestViewModel, IMediator mediator)
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
                Status = RequestStatus.UnAssigned,
                Latitude = requestViewModel.Latitude,
                Longitude = requestViewModel.Longitude
            };

            RequestStatus status;
            if (RequestStatus.TryParse(requestViewModel.Status, out status))
            {
                request.Status = status;
            }

            return request;
        }

    }
}