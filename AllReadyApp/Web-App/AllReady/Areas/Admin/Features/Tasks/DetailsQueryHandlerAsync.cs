using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DetailsQueryHandlerAsync : IAsyncRequestHandler<DetailsQueryAsync, DetailsViewModel>
    {
        private readonly AllReadyContext _context;

        public DetailsQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<DetailsViewModel> Handle(DetailsQueryAsync message)
        {
            var task = await GetTask(message);

            var model = new DetailsViewModel
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                StartDateTime = task.StartDateTime,
                EndDateTime = task.EndDateTime,
                NumberOfVolunteersRequired = task.NumberOfVolunteersRequired,
                EventId = task.Event.Id,
                EventName = task.Event.Name,
                CampaignId = task.Event.CampaignId,
                CampaignName = task.Event.Campaign.Name,
                TimeZoneId = task.Event.Campaign.TimeZoneId,
                RequiredSkills = task.RequiredSkills,
                AssignedVolunteers = task.AssignedVolunteers.Select(ts => new VolunteerViewModel { UserId = ts.User.Id, UserName = ts.User.UserName, HasVolunteered = true }).ToList(),
            };

            foreach (var assignedVolunteer in model.AssignedVolunteers)
            {
                var v = model.AllVolunteers.Single(vvm => vvm.UserId == assignedVolunteer.UserId);
                v.HasVolunteered = true;
            }
            return model;
        }

        private async Task<AllReadyTask> GetTask(DetailsQueryAsync message)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Include(t => t.Event)
                .Include(t => t.Event.Campaign)
                .Include(t => t.AssignedVolunteers).ThenInclude(ts => ts.User)
                .Include(t => t.RequiredSkills).ThenInclude(ts => ts.Skill).ThenInclude(s => s.ParentSkill).ThenInclude(s => s.ParentSkill)
                .SingleOrDefaultAsync(t => t.Id == message.TaskId)
                .ConfigureAwait(false);
        }
    }
}