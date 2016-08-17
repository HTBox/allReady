using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskQueryHandlerAsync : IAsyncRequestHandler<EditTaskQueryAsync, EditViewModel>
    {
        private AllReadyContext _context;

        public EditTaskQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EditViewModel> Handle(EditTaskQueryAsync message)
        {
            return await _context.Tasks.AsNoTracking()
                .Include(t => t.Event).ThenInclude(a => a.Campaign)
                .Include(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .Select(task => new EditViewModel
                {
                    Id = task.Id,
                    Name = task.Name,
                    Description = task.Description,
                    StartDateTime = task.StartDateTime,
                    EndDateTime = task.EndDateTime,
                    NumberOfVolunteersRequired = task.NumberOfVolunteersRequired,
                    RequiredSkills = task.RequiredSkills,
                    EventId = task.Event.Id,
                    EventName = task.Event.Name,
                    CampaignId = task.Event.CampaignId,
                    EventStartDateTime = task.Event.Campaign.StartDateTime,
                    EventEndDateTime = task.Event.Campaign.EndDateTime,
                    CampaignName = task.Event.Campaign.Name,
                    OrganizationId = task.Event.Campaign.ManagingOrganizationId,
                    TimeZoneId = task.Event.Campaign.TimeZoneId
                })
                .SingleAsync(t => t.Id == message.TaskId)
                .ConfigureAwait(false);
        }
    }
}