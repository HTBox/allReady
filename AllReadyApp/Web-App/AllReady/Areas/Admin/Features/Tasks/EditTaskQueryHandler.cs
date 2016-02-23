using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskQueryHandler : IRequestHandler<EditTaskQuery, TaskEditModel>
    {
        private AllReadyContext _context;

        public EditTaskQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public TaskEditModel Handle(EditTaskQuery message)
        {
            var task = _context.Tasks.AsNoTracking()
                .Include(t => t.Activity).ThenInclude(a => a.Campaign)
                .Include(t => t.RequiredSkills)
                .SingleOrDefault(t => t.Id == message.TaskId);
            var viewModel = new TaskEditModel()
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
