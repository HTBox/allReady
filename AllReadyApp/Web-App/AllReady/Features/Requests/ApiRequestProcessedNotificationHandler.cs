using AllReady.Models;
using MediatR;
using System.Linq;
using AllReady.Hangfire.Jobs;
using Hangfire;

namespace AllReady.Features.Requests
{
    //TODO: the behavior of this entire class is "todo". Waiting for feedback from GASA folks
    public class ApiRequestProcessedNotificationHandler : INotificationHandler<ApiRequestProcessedNotification>
    {
        private readonly AllReadyContext context;
        private readonly IBackgroundJobClient backgroundJobClient;

        public ApiRequestProcessedNotificationHandler(AllReadyContext context, IBackgroundJobClient backgroundJobClient)
        {
            this.context = context;
            this.backgroundJobClient = backgroundJobClient;
        }

        public void Handle(ApiRequestProcessedNotification notification)
        {
            var request = context.Requests.SingleOrDefault(x => x.ProviderRequestId == notification.ProviderRequestId);

            //acceptance is true if we can service the Request or false if can't service it
            //TODO mgmccarthy:for now, until we can decide which acceptance status to send GASA when we creat an API Request and asisgn it an orgId, we'll send false for everything
            backgroundJobClient.Enqueue<ISendRequestStatusToGetASmokeAlarm>(x => x.Send(notification.ProviderRequestId, "new", false));
        }
    }
}