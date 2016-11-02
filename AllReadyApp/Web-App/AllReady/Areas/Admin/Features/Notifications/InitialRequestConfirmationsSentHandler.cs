using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Extensions;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class InitialRequestConfirmationsSentHandler : IAsyncNotificationHandler<InitialRequestConfirmationsSent>
    {
        private readonly AllReadyContext context;
        private readonly IBackgroundJobClient backgroundJob;

        public InitialRequestConfirmationsSentHandler(AllReadyContext context, IBackgroundJobClient backgroundJob)
        {
            this.context = context;
            this.backgroundJob = backgroundJob;
        }

        public async Task Handle(InitialRequestConfirmationsSent notification)
        {
            var requests = await context.Requests.Where(x => notification.RequestIds.Contains(x.RequestId)).ToListAsync();
            requests.ForEach(request => request.Status = RequestStatus.PendingConfirmation);
            await context.SaveChangesAsync();
            
            var itinerary = await context.Itineraries.SingleAsync(x => x.Id == notification.ItineraryId);
            
            //TODO mgmccarthy: need to convert itinerary.Date to local time of request's itinerary's campaign's timezoneid
            backgroundJob.Schedule<ISendRequestConfirmationMessagesAWeekBeforeAnItineraryDate>(x => x.SendSms(notification.RequestIds, itinerary.Id), SevenDaysBefore(itinerary.Date));
        }

        private static DateTime SevenDaysBefore(DateTime itineraryDate)
        {
            return itineraryDate.Date.AddDays(-7).AtNoon();
        }
    }
}