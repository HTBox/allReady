using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Models;
using Hangfire;
using MediatR;
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
            //if the requestor is confirming the request "Y", Enqueue changeRequestStatus.To(RequestStatus.Assigned, requestId)
            //if the requestor is canceling the request "N", Enqueue changeRequestStatus.To(RequestStatus.Assigned, requestId)
            //jobClient.Enqueue(() => changeRequestStatus.To(RequestStatus.Assigned, requestId));
            return Ok();
        }
    }

    public interface IChangeRequestStatus
    {
        void To(RequestStatus requestStatus, Guid requestId);
    }

    public class ChangeRequestStatues : IChangeRequestStatus
    {
        private readonly AllReadyContext context;
        private readonly IMediator mediator;

        public ChangeRequestStatues(AllReadyContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public void To(RequestStatus requestStatus, Guid requestId)
        {
            var request = context.Requests.Single(x => x.RequestId == requestId);
            if (requestStatus == RequestStatus.Assigned)
            {
                request.Status = RequestStatus.Assigned;
            }

            if (requestStatus == RequestStatus.Unassigned)
            {
                request.Status = RequestStatus.Unassigned;
                mediator.Send(new SetRequstsToUnassignedCommand { RequestIds = new List<Guid> { requestId } });
            }

            context.SaveChanges();
        }
    }
}