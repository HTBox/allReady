using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.RequestConfirmationMessageSenders;
using AllReady.Extensions;
using AllReady.Models;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class RequestConfirmationsSentHandler : IAsyncNotificationHandler<RequestConfirmationsSent>
    {
        private readonly AllReadyContext context;
        private readonly IBackgroundJobClient backgroundJob;

        public RequestConfirmationsSentHandler(AllReadyContext context, IBackgroundJobClient backgroundJob)
        {
            this.context = context;
            this.backgroundJob = backgroundJob;
        }

        public async Task Handle(RequestConfirmationsSent notification)
        {
            var requests = await context.Requests.Where(x => notification.RequestIds.Contains(x.RequestId)).ToListAsync();
            requests.ForEach(request => request.Status = RequestStatus.PendingConfirmation);
            await context.SaveChangesAsync();
            
            var itinerary = await context.Itineraries.SingleAsync(x => x.Id == notification.ItineraryId);
            
            //TODO mgmccarthy: need to convert intinerary.Date to local time of requestor
            backgroundJob.Schedule<IWeekBeforeRequestConfirmationMessageSender>(x => x.SendSms(notification.RequestIds, itinerary.Id), itinerary.Date.AddDays(-7).AtNoon());
        }
    }
}