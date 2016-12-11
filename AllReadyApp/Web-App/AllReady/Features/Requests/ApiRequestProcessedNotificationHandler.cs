using AllReady.Models;
using MediatR;
using System.Linq;
using AllReady.Hangfire.Jobs;
using Hangfire;

namespace AllReady.Features.Requests
{
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
            //TODO mgmccarthy: insert code here that will determine whether or not we can service the request
            var request = context.Requests.SingleOrDefault(x => x.ProviderRequestId == notification.ProviderRequestId);

            //acceptance is true if we can service the Request or false if can't service it
            backgroundJobClient.Enqueue<ISendRequestStatusToGetASmokeAlarm>(x => x.Send(notification.ProviderRequestId, "new", true));
        }
    }
}