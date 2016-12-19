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
            //TODO mgmccarthy: insert the list of regions here for v1 launch that will allow us to determine if we can service the request or not

            var request = context.Requests.SingleOrDefault(x => x.RequestId == notification.RequestId);
            //acceptance is true if we can service the Request or false if can't service it
            backgroundJobClient.Enqueue<ISendRequestStatusToGetASmokeAlarm>(x => x.Send(request.ProviderRequestId, "new", true));
        }
    }
}