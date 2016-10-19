using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
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
            //update the Request status to PendingConfirmation (this is still TDB functionality)
            var requests = await context.Requests.AsAsyncEnumerable()
                .Where(x => notification.RequestIds.Contains(x.RequestId))
                .ToList();
            requests.ForEach(request => request.Status = RequestStatus.PendingConfirmation);
            await context.SaveChangesAsync();
            
            var itinerary = await context.Itineraries.SingleAsync(x => x.Id == notification.ItineraryId);
            
            //TODO mgmccarthy: need to convert intinerary.Date to local time of requestor
            backgroundJob.Schedule<IRequestConfirmationSmsResender>(x => x.ResendSms(notification.RequestIds, itinerary.Id), SevenDaysBeforeAtNoon(itinerary.Date));
        }

        private static DateTime SevenDaysBeforeAtNoon(DateTime itineraryDate)
        {
            var sevenDaysBefore = itineraryDate.AddDays(-7);
            var sevenDaysBeforeAtNoon = new DateTime(sevenDaysBefore.Year, sevenDaysBefore.Month, sevenDaysBefore.Day, 12, 0, 0);
            return sevenDaysBeforeAtNoon;

            //    var convertedDateTimeOffset = dateTimeOffsetConverter.ConvertDateTimeOffsetTo(campaignsTimeZoneId, dateAssigned, 12, 0, 0);
            //    return convertedDateTimeOffset.AddDays(-7);
        }
    }
}