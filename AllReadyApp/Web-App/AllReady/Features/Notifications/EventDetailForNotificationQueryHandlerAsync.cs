using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Features.Notifications
{
    public class EventDetailForNotificationQueryHandlerAsync : IAsyncRequestHandler<EventDetailForNotificationQueryAsync, EventDetailForNotificationModel>
    {
        private AllReadyContext _context;

        public EventDetailForNotificationQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EventDetailForNotificationModel> Handle(EventDetailForNotificationQueryAsync message)
        {
            EventDetailForNotificationModel result = null;

            var campaignEvent = await GetEvent(message).ConfigureAwait(false);
            var volunteer = await _context.Users.SingleAsync(u => u.Id == message.UserId).ConfigureAwait(false);
            
            if (campaignEvent != null)
            {
                result = new EventDetailForNotificationModel
                {
                    EventId = campaignEvent.Id,
                    EventType = campaignEvent.EventType,
                    CampaignName = campaignEvent.Campaign.Name,
                    CampaignContacts = campaignEvent.Campaign.CampaignContacts,
                    Volunteer = volunteer,
                    EventName = campaignEvent.Name,
                    Description = campaignEvent.Description,
                    UsersSignedUp = campaignEvent.UsersSignedUp,
                    NumberOfVolunteersRequired = campaignEvent.NumberOfVolunteersRequired,
                    Tasks = campaignEvent.Tasks.Select(t => new TaskSummaryModel
                    {
                        Id = t.Id,
                        Name = t.Name,
                        StartDateTime = t.StartDateTime,
                        EndDateTime = t.EndDateTime,
                        NumberOfVolunteersRequired = t.NumberOfVolunteersRequired,
                        AssignedVolunteers = t.AssignedVolunteers.Select(assignedVolunteer => new VolunteerModel
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
                };
            }

            return result;
        }

        private async Task<Models.Event> GetEvent(EventDetailForNotificationQueryAsync message)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(a => a.Campaign)
                .Include(a => a.Campaign.CampaignContacts).ThenInclude(c => c.Contact)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(av => av.User)
                .Include(a => a.UsersSignedUp).ThenInclude(a => a.User)
                .SingleOrDefaultAsync(a => a.Id == message.EventId).ConfigureAwait(false);
        }
    }
}