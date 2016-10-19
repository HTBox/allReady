using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore.Extensions.Internal;

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

            var intineraryRequests = await context.ItineraryRequests.AsAsyncEnumerable()
                .Where(x => notification.RequestIds.Contains(x.RequestId))
                .ToList();

            //var campaignsTimeZoneId = context.Itineraries.AsNoTracking()
            //    .Include(i => i.Event).ThenInclude(e => e.Campaign)
            //    .Single(i => i.Id == notification.ItineraryId).Event.Campaign.TimeZoneId;

            //TODO mgmccarthy: do we want to handle these requests as a batch or schedule them per request?
            //TODO mgmccarthy: need to convert DateAssigned to local time of requestor
            //intineraryRequests.ForEach(request => backgroundJob.Schedule<ISmsRequestConfirmationResender>(x => 
            //    x.ResendSms(request.RequestId), SevenDaysBeforeAtNoon(request.DateAssigneDateTimeOffset, campaignsTimeZoneId)));

            intineraryRequests.ForEach(request => backgroundJob.Schedule<ISmsRequestConfirmationResender>(x =>
                x.ResendSms(request.RequestId), SevenDaysBeforeAtNoon(request.DateAssigned)));
            //intineraryRequests.ForEach(request => backgroundJob.Schedule<ISmsRequestConfirmationResender>(x =>
            //    x.ResendSms(request.RequestId), TimeSpan.FromSeconds(15)));
        }

        private static DateTime SevenDaysBeforeAtNoon(DateTime dateAssigned)
        {
            var sevenDaysBefore = dateAssigned.AddDays(-7);
            var sevenDaysBeforeAtNoon = new DateTime(sevenDaysBefore.Year, sevenDaysBefore.Month, sevenDaysBefore.Day, 12, 0, 0);
            return sevenDaysBeforeAtNoon;
        }

        //private DateTimeOffset SevenDaysBeforeAtNoon(DateTimeOffset dateAssigned, string campaignsTimeZoneId)
        //{
        //    var convertedDateTimeOffset = dateTimeOffsetConverter.ConvertDateTimeOffsetTo(campaignsTimeZoneId, dateAssigned, 12, 0, 0);
        //    return convertedDateTimeOffset.AddDays(-7);
        //}
    }
}