using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models.ItineraryModels;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventDetailQueryHandler : IAsyncRequestHandler<EventDetailQuery, EventDetailModel>
    {
        private readonly AllReadyContext _context;

        public EventDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EventDetailModel> Handle(EventDetailQuery message)
        {
            EventDetailModel result = null;

            var campaignEvent = await GetEvent(message);

            if (campaignEvent != null)
            {
                result = new EventDetailModel
                {
                    Id = campaignEvent.Id,
                    EventType = campaignEvent.EventType,
                    CampaignName = campaignEvent.Campaign.Name,
                    CampaignId = campaignEvent.Campaign.Id,
                    OrganizationId = campaignEvent.Campaign.ManagingOrganizationId,
                    OrganizationName = campaignEvent.Campaign.ManagingOrganization.Name,
                    Name = campaignEvent.Name,
                    Description = campaignEvent.Description,
                    TimeZoneId = campaignEvent.Campaign.TimeZoneId,
                    StartDateTime = campaignEvent.StartDateTime,
                    EndDateTime = campaignEvent.EndDateTime,
                    Volunteers = campaignEvent.UsersSignedUp.Select(u => u.User.UserName).ToList(),
                    NumberOfVolunteersRequired = campaignEvent.NumberOfVolunteersRequired,
                    IsLimitVolunteers = campaignEvent.IsLimitVolunteers,
                    IsAllowWaitList = campaignEvent.IsAllowWaitList,
                    Location = campaignEvent.Location.ToEditModel(),
                    RequiredSkills = campaignEvent.RequiredSkills,
                    ImageUrl = campaignEvent.ImageUrl,
                    Tasks = campaignEvent.Tasks.Select(t => new TaskSummaryModel
                    {
                        Id = t.Id,
                        Name = t.Name,
                        StartDateTime = t.StartDateTime,
                        EndDateTime = t.EndDateTime,
                        NumberOfVolunteersRequired = t.NumberOfVolunteersRequired,
                        AssignedVolunteers = t.AssignedVolunteers?.Select(assignedVolunteer => new VolunteerModel
                        {
                            UserId = assignedVolunteer.User.Id,
                            UserName = assignedVolunteer.User.UserName,
                            HasVolunteered = true,
                            Status = assignedVolunteer.Status,
                            PreferredEmail = assignedVolunteer.PreferredEmail,
                            PreferredPhoneNumber = assignedVolunteer.PreferredPhoneNumber,
                            AdditionalInfo = assignedVolunteer.AdditionalInfo
                        }).ToList()
                    }).OrderBy(t => t.StartDateTime).ThenBy(t => t.Name).ToList(),
                    Itineraries = campaignEvent.Itineraries.Select(i => new ItineraryListModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Date = i.Date,
                        TeamSize = i.TeamMembers.Count,
                        RequestCount = i.Requests.Count
                    }).OrderBy(i => i.Date).ToList()
                };

                result.NewItinerary.EventId = result.Id;
                result.NewItinerary.Date = result.StartDateTime.DateTime;
            }

            return result;
        }

        private async Task<Event> GetEvent(EventDetailQuery message)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(av => av.User)
                .Include(a => a.RequiredSkills).ThenInclude(s => s.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.Location)
                .Include(a => a.UsersSignedUp).ThenInclude(a => a.User)
                .Include(a => a.Itineraries).ThenInclude(a => a.TeamMembers)
                .Include(a => a.Itineraries).ThenInclude(a => a.Requests)
                .SingleOrDefaultAsync(a => a.Id == message.EventId)
                .ConfigureAwait(false);
        }
    }
}