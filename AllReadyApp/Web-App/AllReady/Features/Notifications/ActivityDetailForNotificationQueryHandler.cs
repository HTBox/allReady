using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Features.Notifications
{
    public class ActivityDetailForNotificationQueryHandler : IRequestHandler<ActivityDetailForNotificationQuery, ActivityDetailForNotificationModel>
    {
        private AllReadyContext _context;

        public ActivityDetailForNotificationQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public ActivityDetailForNotificationModel Handle(ActivityDetailForNotificationQuery message)
        {
            ActivityDetailForNotificationModel result = null;

            var activity = GetActivity(message);
            var volunteer = _context.Users.Single(u => u.Id == message.UserId);
            
            if (activity != null)
            {
                result = new ActivityDetailForNotificationModel
                {
                    ActivityId = activity.Id,
                    ActivityType = activity.ActivityType,
                    CampaignName = activity.Campaign.Name,
                    CampaignContacts = activity.Campaign.CampaignContacts,
                    Volunteer = volunteer,
                    ActivityName = activity.Name,
                    Description = activity.Description,
                    UsersSignedUp = activity.UsersSignedUp,
                    NumberOfVolunteersRequired = activity.NumberOfVolunteersRequired,
                    Tasks = activity.Tasks.Select(t => new TaskSummaryModel()
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

        private Models.Activity GetActivity(ActivityDetailForNotificationQuery message)
        {
            return _context.Activities
                .AsNoTracking()
                .Include(a => a.Campaign)
                .Include(a => a.Campaign.CampaignContacts).ThenInclude(c => c.Contact)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(av => av.User)
                .Include(a => a.UsersSignedUp).ThenInclude(a => a.User)
                .SingleOrDefault(a => a.Id == message.ActivityId);
        }
    }
}