using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using Hangfire;
using MediatR;
using TimeZoneConverter;
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

            var itinerary = await context.Itineraries.Include(i => i.Event).SingleAsync(x => x.Id == notification.ItineraryId);

            backgroundJob.Schedule<ISendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate>(x => x.SendSms(notification.RequestIds, itinerary.Id), SevenDaysBeforeThe(itinerary.Date, itinerary.Event.TimeZoneId));
        }

        private static DateTimeOffset SevenDaysBeforeThe(DateTime itineraryDate, string eventsTimeZoneId)
        {
            var sevenDaysBeforeTheIntinerayDateAtNoon = itineraryDate.Date.AddDays(-7).AddHours(12);
            var timeZoneInfo = TZConvert.GetTimeZoneInfo(eventsTimeZoneId);
            var utcOffset = timeZoneInfo.GetUtcOffset(sevenDaysBeforeTheIntinerayDateAtNoon);
            return new DateTimeOffset(sevenDaysBeforeTheIntinerayDateAtNoon, utcOffset);
        }
    }
}
