using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Requests
{
    public class ApiRequestProcessedNotificationHandler : IAsyncNotificationHandler<ApiRequestProcessedNotification>
    {
        private readonly AllReadyContext context;
        private readonly IMediator mediator;

        public ApiRequestProcessedNotificationHandler(AllReadyContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public async Task Handle(ApiRequestProcessedNotification notification)
        {
            //b/c we might not have created the Request b/c we could not service it b/c we do not have a matching zip code in any of our Events, we query for Request by ProviderId, not RequestId
            var request = await context.Requests.SingleOrDefaultAsync(x => x.ProviderId == notification.ProviderRequestId);

            await mediator.SendAsync(new SendRequestStatusToGetASmokeAlarm
            {
                Serial = notification.ProviderRequestId,
                Status = "new",
                //true if we've decided to service the Request (meaning we wrote it to the Request table with a matching Event by zip code), or false if we've decided we can't service it (meaning we could find no zip code match on any of our Events)
                Acceptance = request != null 
            });
        }
    }
}