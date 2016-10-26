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
            //look up the Requestor by RequestorId on incoming sms messages
            //we cannot look up Requestor by Phone b/c one Requestor could have multiple Requests in allReady for the same intinerary date (??? actually, need to ask project owners about this)
            //if the requestor is confirming the request "Y": jobClient.Enqueue(() => changeRequestStatus.To(RequestStatus.Confirmed, requestId));
            //if the requestor is canceling the request "N", jobClient.Enqueue(() => changeRequestStatus.To(RequestStatus.Unassigned, requestId));

            DateTime dateTime;
            string message;
            //30 seconds from now
            //dateTime = DateTimeOffset.Now.Date.AddSeconds(30);
            //message = "this should execute in 30 seconds";

            //yesterday
            dateTime = DateTimeOffset.Now.Date.AddDays(-1);
            message = "this should execute immediately";

            //I just proved that scheduling a job in the past results in it being executed immediately
            jobClient.Schedule(() => Debug.WriteLine(message), dateTime);
            return Ok();
        }
    }
}