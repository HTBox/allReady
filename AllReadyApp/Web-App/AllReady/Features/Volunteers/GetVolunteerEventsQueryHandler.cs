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
    public class GetVolunteerEventsQueryHandler : IAsyncRequestHandler<GetVolunteerEventsQuery, MyEventsListerViewModel>
    {
        private AllReadyContext _context;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public GetVolunteerEventsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }
        
        public async Task<MyEventsListerViewModel> Handle(GetVolunteerEventsQuery message)
        {
            var volunteerTaskSignups = await _context.VolunteerTaskSignups.AsNoTracking()
                .Include(rec => rec.VolunteerTask).ThenInclude(rec => rec.Event).ThenInclude(rec => rec.Campaign).ThenInclude(rec => rec.ManagingOrganization)
                .Include(rec => rec.User)
                .Select(rec => new
                {
                    EventId = rec.VolunteerTask.Event.Id,
                    EventName = rec.VolunteerTask.Event.Name,
                    EventStartDate = rec.VolunteerTask.Event.StartDateTime,
                    EventEndDate = rec.VolunteerTask.Event.EndDateTime,
                    TimeZone = rec.VolunteerTask.Event.Campaign.TimeZoneId,
                    TaskName = rec.VolunteerTask.Name,
                    TaskStartDate = rec.VolunteerTask.StartDateTime,
                    TaskEndDate = rec.VolunteerTask.EndDateTime,
                    CampaignName = rec.VolunteerTask.Event.Campaign.Name,
                    OrganizationName = rec.VolunteerTask.Event.Campaign.ManagingOrganization.Name      ,
                    VolunteerCount = rec.VolunteerTask.AssignedVolunteers.Count
                })
                .ToListAsync();

            var results = new List<MyEventsListerItem>();

            foreach (var rec in volunteerTaskSignups.GroupBy(t => t.EventId))
            {
                var eventItem = new MyEventsListerItem
                {
                    EventId = rec.Key,
                    EventName = rec.FirstOrDefault().EventName,
                    StartDate = rec.FirstOrDefault().EventStartDate,
                    EndDate = rec.FirstOrDefault().EventEndDate,
                    TimeZone = rec.FirstOrDefault().TimeZone,
                    Campaign = rec.FirstOrDefault().CampaignName,
                    Organization = rec.FirstOrDefault().OrganizationName,
                    VolunteerCount = rec.FirstOrDefault().VolunteerCount
                };

                results.Add(eventItem);
            }

            foreach(var campaignEvent in results)
            {
                var signups = volunteerTaskSignups.Where(t => t.EventId == campaignEvent.EventId);

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
