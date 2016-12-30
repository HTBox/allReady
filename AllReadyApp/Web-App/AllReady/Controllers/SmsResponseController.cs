using AllReady.Attributes;
using AllReady.Features.Requests;
using AllReady.Hangfire.Jobs;
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
        private const string Accepted = "y";
        private const string Declined = "n";

        private readonly IChangeRequestStatus _changeRequestStatus;
        private readonly IMediator _mediator;
        private readonly ISmsSender _smsSender;

        public SmsResponseController(IChangeRequestStatus changeRequestStatus, IMediator mediator, ISmsSender smsSender)
        {
            _changeRequestStatus = changeRequestStatus;
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
                        case (Accepted):
                            _changeRequestStatus.To(RequestStatus.Confirmed, requestId); // this method is not async and uses non async db access also - leaving this as is whilst we refine the requirements past v1
                            await _smsSender.SendSmsAsync(from, "Thank you for confirming you availability.");
                            break;

                        case (Declined):
                            _changeRequestStatus.To(RequestStatus.Unassigned, requestId); // this method is not async and uses non async db access also - leaving this as is whilst we refine the requirements past v1
                            await _smsSender.SendSmsAsync(from, "We have canceled your request and once it is rescheduled you will receive further communication.");
                            break;

                        default:
                            // log this
                            break;
                    }

                    return Ok();
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