using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DetailsQueryHandler : IAsyncRequestHandler<DetailsQuery, DetailsViewModel>
    {
        private readonly AllReadyContext _context;

        public DetailsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<DetailsViewModel> Handle(DetailsQuery message)
        {
            var theTask = await GetTask(message);

            var model = new DetailsViewModel
            {
                Id = theTask.Id,
                Name = theTask.Name,
                Description = theTask.Description,
                StartDateTime = theTask.StartDateTime,
                EndDateTime = theTask.EndDateTime,
                NumberOfVolunteersRequired = theTask.NumberOfVolunteersRequired,
                EventId = theTask.Event.Id,
                EventName = theTask.Event.Name,
                CampaignId = theTask.Event.CampaignId,
                CampaignName = theTask.Event.Campaign.Name,
                TimeZoneId = theTask.Event.TimeZoneId,
                RequiredSkills = theTask.RequiredSkills,
                AssignedVolunteers = theTask.AssignedVolunteers.Select(ts => new VolunteerViewModel
                {
                    UserId = ts.User.Id,
                    UserName = ts.User.UserName,
                    HasVolunteered = true,
                    Name = ts.User.Name,
                    PhoneNumber = ts.User.PhoneNumber,
                    AssociatedSkills = ts.User.AssociatedSkills,
                }).ToList(),
            };

            return model;
        }

        private async Task<AllReadyTask> GetTask(DetailsQuery message)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Include(t => t.Event)
                .Include(t => t.Event.Campaign)
                .Include(t => t.AssignedVolunteers).ThenInclude(ts => ts.User).ThenInclude(u => u.AssociatedSkills).ThenInclude(s => s.Skill).ThenInclude(s => s.ParentSkill).ThenInclude(s => s.ParentSkill)
                .Include(t => t.RequiredSkills).ThenInclude(ts => ts.Skill).ThenInclude(s => s.ParentSkill).ThenInclude(s => s.ParentSkill)
                .SingleOrDefaultAsync(t => t.Id == message.TaskId);
        }
    }
}