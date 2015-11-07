using AllReady.Areas.Admin.ViewModels;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class ActivityDetailQueryHandler : IRequestHandler<ActivityDetailQuery, ActivityDetailViewModel>
    {
        private AllReadyContext _context;

        public ActivityDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public ActivityDetailViewModel Handle(ActivityDetailQuery message)
        {
            ActivityDetailViewModel result = null;

            var activity = _context.Activities
                .AsNoTracking()
                .Include(a => a.Campaign)
                .Include(a => a.Tasks)
                .Include(a => a.RequiredSkills)
                .Include(a => a.UsersSignedUp).ThenInclude(a => a.User)
                .SingleOrDefault(a => a.Id == message.ActivityId);

            if (activity != null)
            {
                result = new ActivityDetailViewModel
                {
                    Id = activity.Id,
                    CampaignName = activity.Campaign.Name,
                    CampaignId = activity.Campaign.Id,
                    Name = activity.Name,
                    Description = activity.Description,
                    StartDateTime = activity.StartDateTimeUtc,
                    EndDateTime = activity.EndDateTimeUtc,
                    Volunteers = activity.UsersSignedUp.Select(u => u.User.UserName).ToList(),
                    Tasks = activity.Tasks.Select(t => new TaskSummaryViewModel()
                    {
                        Id = t.Id,
                        Name = t.Name,
                        StartDateTime = t.StartDateTimeUtc,
                        EndDateTime = t.EndDateTimeUtc,
                    }).OrderBy(t => t.StartDateTime).ThenBy(t => t.Name).ToList(),
                    RequiredSkills = activity.RequiredSkills,
                    ImageUrl = activity.ImageUrl
                };
            }
            return result;
        }
    }
}
