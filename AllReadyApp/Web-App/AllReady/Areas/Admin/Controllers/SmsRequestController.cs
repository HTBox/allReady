using System;
using System.Diagnostics;
using AllReady.Hangfire.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    public class SmsRequestController : Controller
    {
        private readonly IBackgroundJobClient jobClient;
        private readonly IChangeRequestStatus changeRequestStatus;

        public SmsRequestController(IBackgroundJobClient jobClient, IChangeRequestStatus changeRequestStatus)
        {
            this.jobClient = jobClient;
            this.changeRequestStatus = changeRequestStatus;
        }

        public IActionResult Index()
        {
            //get RequestId from incoming sms message
            //we cannot look up RequestId by PhoneNumber b/c one Requestor could have multiple Requests in allReady for the same itinerary date (??? actually, need to ask project owners about this)
            //if the requestor is confirming the request "Y": jobClient.Enqueue(() => changeRequestStatus.To(RequestStatus.Confirmed, requestId));
            //if the requestor is canceling the request "N", jobClient.Enqueue(() => changeRequestStatus.To(RequestStatus.Unassigned, requestId));

            //TODO mgmccarthy: If the incoming response is "Y"/"N", send a message to the requestor? Need to ask project owners
            return Ok();
        }
    }
}