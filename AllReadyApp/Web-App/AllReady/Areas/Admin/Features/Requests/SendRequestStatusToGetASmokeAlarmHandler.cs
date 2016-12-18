using System.Threading.Tasks;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class SendRequestStatusToGetASmokeAlarmHandler : IAsyncNotificationHandler<RequestStatusChangedNotification>
    {
        private readonly AllReadyContext context;
        private readonly IBackgroundJobClient backgroundJobClient;

        public SendRequestStatusToGetASmokeAlarmHandler(AllReadyContext context, IBackgroundJobClient backgroundJobClient)
        {
            this.context = context;
            this.backgroundJobClient = backgroundJobClient;
        }

        public async Task Handle(RequestStatusChangedNotification notification)
        {
            if (await context.Requests.AnyAsync(x => x.RequestId == notification.RequestId && x.Source == RequestSource.Api))
            {
                if (notification.NewStatus == RequestStatus.Completed || notification.NewStatus == RequestStatus.Canceled || notification.NewStatus == RequestStatus.Assigned || notification.NewStatus == RequestStatus.Unassigned)
                {
                    var gasaStatus = string.Empty;
                    var acceptance = false;
                    switch (notification.NewStatus)
                    {
                        case RequestStatus.Completed:
                            gasaStatus = "installed";
                            acceptance = true;
                            break;
                        case RequestStatus.Canceled:
                            gasaStatus = "canceled";
                            acceptance = true;
                            break;
                        case RequestStatus.Assigned:
                            gasaStatus = "in progress";
                            acceptance = true;
                            break;
                        //???
                        case RequestStatus.Unassigned:
                            gasaStatus = "new";
                            acceptance = false;
                            break;
                        //???
                    }

                    var request = await context.Requests.SingleAsync(x => notification.RequestId == x.RequestId && x.Source == RequestSource.Api);
                    //backgroundJobClient.Enqueue<ISendRequestStatusToGetASmokeAlarm>(x => x.Send(request.ProviderRequestId, gasaStatus, true));
                    backgroundJobClient.Enqueue<ISendRequestStatusToGetASmokeAlarm>(x => x.Send(request.ProviderRequestId, gasaStatus, acceptance));
                }
            }
        }
    }
}