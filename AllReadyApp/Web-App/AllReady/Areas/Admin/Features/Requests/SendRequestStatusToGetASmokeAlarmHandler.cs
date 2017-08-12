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
            var request = await context.Requests.SingleOrDefaultAsync(x => notification.RequestId == x.RequestId && x.Source == RequestSource.Api);
            if (request != null)
            {
                if (notification.NewStatus == RequestStatus.Completed || notification.NewStatus == RequestStatus.Canceled || notification.NewStatus == RequestStatus.Assigned || notification.NewStatus == RequestStatus.Unassigned || notification.NewStatus == RequestStatus.Requested)
                {
                    var gasaStatus = string.Empty;
                    var acceptance = false;

                    switch (notification.NewStatus)
                    {
                        case RequestStatus.Completed:
                            gasaStatus = GasaStatus.Installed;
                            acceptance = true;
                            break;
                        case RequestStatus.Canceled:
                            gasaStatus = GasaStatus.Canceled;
                            acceptance = true;
                            break;
                        case RequestStatus.Assigned:
                            gasaStatus = GasaStatus.InProgress;
                            acceptance = true;
                            break;
                        case RequestStatus.Unassigned:
                            gasaStatus = GasaStatus.New;
                            break;
                        case RequestStatus.Requested:
                            gasaStatus = GasaStatus.Requested;
                            break;
                    }

                    backgroundJobClient.Enqueue<ISendRequestStatusToGetASmokeAlarm>(x => x.Send(request.ProviderRequestId, gasaStatus, acceptance));
                }
            }
        }
    }
}