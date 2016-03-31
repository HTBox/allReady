using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskQueryHandlerAsync : IAsyncRequestHandler<EditTaskQueryAsync, TaskEditModel>
    {
        private AllReadyContext _context;

        public EditTaskQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<TaskEditModel> Handle(EditTaskQueryAsync message)
        {
            var task = await _context.Tasks.AsNoTracking()
                .Include(t => t.Activity).ThenInclude(a => a.Campaign)
                .Include(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .SingleOrDefaultAsync(t => t.Id == message.TaskId);

            var viewModel = new TaskEditModel
            {
                Id = task.Id,
                ActivityId = task.Activity.Id,
                ActivityName = task.Activity.Name,
                CampaignId = task.Activity.CampaignId,
                CampaignName = task.Activity.Campaign.Name,
                OrganizationId = task.Activity.Campaign.ManagingOrganizationId,
                Name = task.Name,
                Description = task.Description,
                TimeZoneId = task.Activity.Campaign.TimeZoneId,
                StartDateTime = task.StartDateTime,
                EndDateTime = task.EndDateTime,
                NumberOfVolunteersRequired = task.NumberOfVolunteersRequired,
                RequiredSkills = task.RequiredSkills
            };
                    
            return viewModel;
        }
    }
}
