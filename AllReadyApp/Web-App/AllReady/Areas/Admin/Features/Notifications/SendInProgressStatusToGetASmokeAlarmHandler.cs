using System.Linq;
using System.Threading.Tasks;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class SendInProgressStatusToGetASmokeAlarmHandler : IAsyncNotificationHandler<RequestsAssignedToItinerary>
    {
        private readonly AllReadyContext context;
        private readonly IBackgroundJobClient backgroundJobClient;

        public SendInProgressStatusToGetASmokeAlarmHandler(AllReadyContext context, IBackgroundJobClient backgroundJobClient)
        {
            this.context = context;
            this.backgroundJobClient = backgroundJobClient;
        }

        public async Task Handle(RequestsAssignedToItinerary notification)
        {
            var providerRequestIds = await context.Requests.Where(x => notification.RequestIds.Contains(x.RequestId) && x.Source == RequestSource.Api).Select(x => x.ProviderRequestId).ToListAsync();
            providerRequestIds.ForEach(providerRequestId => backgroundJobClient.Enqueue<ISendRequestStatusToGetASmokeAlarm>(x => x.Send(providerRequestId, GasaStatus.InProgress, true)));
        }
    }
}
