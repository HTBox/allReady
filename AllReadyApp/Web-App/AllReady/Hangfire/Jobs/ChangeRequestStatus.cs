using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Hangfire.MediatR;
using AllReady.Models;
using MediatR;

namespace AllReady.Hangfire.Jobs
{
    public class ChangeRequestStatus : IChangeRequestStatus
    {
        private readonly AllReadyContext context;
        private readonly IMediator mediator;

        public ChangeRequestStatus(AllReadyContext context, IMediator mediator)
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
                mediator.Send(new SetRequestsToUnassignedCommand { RequestIds = new List<Guid> { requestId } });
            }

            context.SaveChanges();
        }
    }

    public interface IChangeRequestStatus : IHangfireJob
    {
        void To(RequestStatus requestStatus, Guid requestId);
    }
}