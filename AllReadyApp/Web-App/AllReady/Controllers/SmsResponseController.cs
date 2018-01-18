using AllReady.Areas.Admin.Features.Requests;
using AllReady.Attributes;
using AllReady.Features.Requests;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AllReady.Controllers
{
    public class SmsResponseController : Controller
    {
        private const string AcceptedResponse = "y";
        private const string DeclinedResponse = "n";

        private readonly IMediator _mediator;
        private readonly ISmsSender _smsSender;

        public SmsResponseController(IMediator mediator, ISmsSender smsSender)
        {
            _mediator = mediator;
            _smsSender = smsSender;
        }

        [HttpPost("smsresponse")]
        [ExternalEndpoint]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string from, string body)
        {
            if (string.IsNullOrWhiteSpace(from))
            { 
                // this should never be the case for legitimate requests from Twilio
                return BadRequest();
            }

            var requestId = await _mediator.SendAsync(new FindRequestIdByPhoneNumberQuery(from));

            if (requestId != Guid.Empty)
            {
                var response = body.ToLower();

                try
                {
                    switch (response)
                    {
                        case (AcceptedResponse):
                            await _mediator.SendAsync(new ChangeRequestStatusCommand { RequestId = requestId, NewStatus = RequestStatus.Confirmed });
                            await _smsSender.SendSmsAsync(from, "Thank you for confirming your availability.");
                            break;

                        case (DeclinedResponse):
                            await _mediator.SendAsync(new ChangeRequestStatusCommand { RequestId = requestId, NewStatus = RequestStatus.Unassigned });
                            await _smsSender.SendSmsAsync(from, "We have canceled your request and once it is rescheduled you will receive further communication.");
                            break;

                        default:
                            // log this
                            break;
                    }
                }
                catch (Exception)
                {
                    // log / handle once we determine how logging will be managed in production

                    return StatusCode(500);
                }
            }
            else
            {
                // todo - store failed phone matches into DB or a log for review / diagnostics - post v1?
            }

            return Ok();
        }
    }
}
