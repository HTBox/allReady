using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class ActivityDetailQueryHandler : IRequestHandler<ActivityDetailQuery, ActivityDetailModel>
    {
        private readonly AllReadyContext _context;

        public ActivityDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public ActivityDetailModel Handle(ActivityDetailQuery message)
        {
            ActivityDetailModel result = null;

            var activity = GetActivity(message);

            if (activity != null)
            {
                result = new ActivityDetailModel
                {
                    Id = activity.Id,
                    ActivityType = activity.ActivityType,
                    CampaignName = activity.Campaign.Name,
                    CampaignId = activity.Campaign.Id,
                    OrganizationId = activity.Campaign.ManagingOrganizationId,
                    OrganizationName = activity.Campaign.ManagingOrganization.Name,
                    Name = activity.Name,
                    Description = activity.Description,
                    TimeZoneId = activity.Campaign.TimeZoneId,
                    StartDateTime = activity.StartDateTime,
                    EndDateTime = activity.EndDateTime,
                    Volunteers = activity.UsersSignedUp.Select(u => u.User.UserName).ToList(),
                    NumberOfVolunteersRequired = activity.NumberOfVolunteersRequired,
                    IsLimitVolunteers = activity.IsLimitVolunteers,
                    IsAllowWaitList = activity.IsAllowWaitList,
                    Location = LocationExtensions.ToEditModel(activity.Location),
                    RequiredSkills = activity.RequiredSkills,
                    ImageUrl = activity.ImageUrl,
                    Tasks = activity.Tasks.Select(t => new TaskSummaryModel()
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
                };
            }

            return result;
        }

        private Activity GetActivity(ActivityDetailQuery message)
        {
            return _context.Activities
                .AsNoTracking()
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(av => av.User)
                .Include(a => a.RequiredSkills).ThenInclude(s => s.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.Location).ThenInclude(p => p.PostalCode)
                .Include(a => a.UsersSignedUp).ThenInclude(a => a.User)
                .SingleOrDefault(a => a.Id == message.ActivityId);
        }
    }
}