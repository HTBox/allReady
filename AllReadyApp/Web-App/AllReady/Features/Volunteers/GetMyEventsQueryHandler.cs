using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Features.Volunteers
{
    public class GetMyEventsQueryHandler : IAsyncRequestHandler<GetMyEventsQuery, MyEventsListerViewModel>
    {
        private AllReadyContext _context;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public GetMyEventsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }
        
        public async Task<MyEventsListerViewModel> Handle(GetMyEventsQuery message)
        {
            var taskSignups = await _context.TaskSignups.AsNoTracking()
                .Include(rec => rec.Task).ThenInclude(rec => rec.Event).ThenInclude(rec => rec.Campaign).ThenInclude(rec => rec.ManagingOrganization)
                .Include(rec => rec.User)
                .Where(rec => rec.User.Id == message.UserId)
                .Select(rec => new
                {
                    EventId = rec.Task.Event.Id,
                    EventName = rec.Task.Event.Name,
                    EventStartDate = rec.Task.Event.StartDateTime,
                    EventEndDate = rec.Task.Event.EndDateTime,
                    TimeZone = rec.Task.Event.Campaign.TimeZoneId,
                    TaskName = rec.Task.Name,
                    TaskStartDate = rec.Task.StartDateTime,
                    TaskEndDate = rec.Task.EndDateTime,
                    CampaignName = rec.Task.Event.Campaign.Name,
                    OrganizationName = rec.Task.Event.Campaign.ManagingOrganization.Name      
                })
                .ToListAsync();

            var results = new List<MyEventsListerItem>();

            foreach (var rec in taskSignups.GroupBy(t => t.EventId))
            {
                var eventItem = new MyEventsListerItem
                {
                    EventId = rec.Key,
                    EventName = rec.FirstOrDefault().EventName,
                    StartDate = rec.FirstOrDefault().EventStartDate,
                    EndDate = rec.FirstOrDefault().EventEndDate,
                    TimeZone = rec.FirstOrDefault().TimeZone,
                    Campaign = rec.FirstOrDefault().CampaignName,
                    Organization = rec.FirstOrDefault().OrganizationName
                };

                results.Add(eventItem);
            }

            foreach(var campaignEvent in results)
            {
                var signups = taskSignups.Where(t => t.EventId == campaignEvent.EventId);

                foreach(var signup in signups)
                {
                    campaignEvent.Tasks.Add(new MyEventsListerItemTask {
                        Name = signup.TaskName,
                        StartDate = signup.TaskStartDate,
                        EndDate = signup.TaskEndDate
                    });
                }
            }

            return new MyEventsListerViewModel
            {
                CurrentEvents = results.Where(r => r.StartDate.UtcDateTime < DateTimeUtcNow() && r.EndDate.UtcDateTime > DateTimeUtcNow())
                    .OrderBy(r => r.StartDate.UtcDateTime).ThenByDescending(r => r.EndDate.UtcDateTime),
                FutureEvents = results.Where(r => r.StartDate.UtcDateTime > DateTimeUtcNow())
                    .OrderBy(r => r.StartDate.UtcDateTime).ThenByDescending(r => r.EndDate.UtcDateTime),
                PastEvents = results.Where(r => r.EndDate.UtcDateTime < DateTimeUtcNow())
                    .OrderBy(r => r.StartDate.UtcDateTime).ThenByDescending(r => r.EndDate.UtcDateTime),
            };
        }
    }
}
