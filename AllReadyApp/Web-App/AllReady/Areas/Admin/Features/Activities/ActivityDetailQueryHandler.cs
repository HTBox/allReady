using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class ActivityDetailQueryHandler : IRequestHandler<ActivityDetailQuery, ActivityDetailModel>
    {
        private AllReadyContext _context;

        public ActivityDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public ActivityDetailModel Handle(ActivityDetailQuery message)
        {
            ActivityDetailModel result = null;

            var activity = _context.Activities
                .AsNoTracking()
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.Tasks)
                .Include(a => a.RequiredSkills)
                .Include(a => a.UsersSignedUp).ThenInclude(a => a.User)
                .SingleOrDefault(a => a.Id == message.ActivityId);

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
                    Tasks = activity.Tasks.Select(t => new TaskSummaryModel()
                    {
                        Id = t.Id,
                        Name = t.Name,
                        StartDateTime = t.StartDateTime,
                        EndDateTime = t.EndDateTime,
                    }).OrderBy(t => t.StartDateTime).ThenBy(t => t.Name).ToList(),
                    RequiredSkills = activity.RequiredSkills,
                    ImageUrl = activity.ImageUrl
                };
            }
            return result;
        }
    }
}