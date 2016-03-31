﻿using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Features.Notifications
{
    public class ActivityDetailForNotificationQueryHandlerAsync : IAsyncRequestHandler<ActivityDetailForNotificationQueryAsync, ActivityDetailForNotificationModel>
    {
        private AllReadyContext _context;

        public ActivityDetailForNotificationQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<ActivityDetailForNotificationModel> Handle(ActivityDetailForNotificationQueryAsync message)
        {
            ActivityDetailForNotificationModel result = null;

            var activity = await GetActivity(message).ConfigureAwait(false);
            var volunteer = await _context.Users.SingleAsync(u => u.Id == message.UserId).ConfigureAwait(false);
            
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
                    Tasks = activity.Tasks.Select(t => new TaskSummaryModel
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

        private async Task<Models.Activity> GetActivity(ActivityDetailForNotificationQueryAsync message)
        {
            return await _context.Activities
                .AsNoTracking()
                .Include(a => a.Campaign)
                .Include(a => a.Campaign.CampaignContacts).ThenInclude(c => c.Contact)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(av => av.User)
                .Include(a => a.UsersSignedUp).ThenInclude(a => a.User)
                .SingleOrDefaultAsync(a => a.Id == message.ActivityId).ConfigureAwait(false);
        }
    }
}