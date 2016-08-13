using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class TaskQueryHandlerAsync : IAsyncRequestHandler<TaskQueryAsync, TaskSummaryViewModel>
    {
        private readonly AllReadyContext _context;

        public TaskQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<TaskSummaryViewModel> Handle(TaskQueryAsync message)
        {
            var task = await GetTask(message);

            var taskModel = new TaskSummaryViewModel()
            {
                Id = task.Id,
                EventId = task.Event.Id,
                EventName = task.Event.Name,
                CampaignId = task.Event.CampaignId,
                CampaignName = task.Event.Campaign.Name,
                OrganizationId = task.Event.Campaign.ManagingOrganizationId,
                Name = task.Name,
                Description = task.Description,
                TimeZoneId = task.Event.Campaign.TimeZoneId,
                StartDateTime = task.StartDateTime,
                EndDateTime = task.EndDateTime,
                NumberOfVolunteersRequired = task.NumberOfVolunteersRequired,
                RequiredSkills = task.RequiredSkills,
                AssignedVolunteers = task.AssignedVolunteers.Select(av => new VolunteerViewModel { UserId = av.User.Id, UserName = av.User.UserName, HasVolunteered = true }).ToList(),
                AllVolunteers = task.Event.UsersSignedUp.Select(v => new VolunteerViewModel { UserId = v.User.Id, UserName = v.User.UserName, HasVolunteered = false }).ToList()
            };

            foreach (var assignedVolunteer in taskModel.AssignedVolunteers)
            {
                var v = taskModel.AllVolunteers.Single(al => al.UserId == assignedVolunteer.UserId);
                v.HasVolunteered = true;
            }
            return taskModel;
        }

        private async Task<AllReadyTask> GetTask(TaskQueryAsync message)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Include(t => t.Event).ThenInclude(a => a.UsersSignedUp).ThenInclude(us => us.User)
                .Include(t => t.Event.Campaign)
                .Include(t => t.AssignedVolunteers).ThenInclude(av => av.User)
                .Include(s => s.RequiredSkills).ThenInclude(s => s.Skill).ThenInclude(s => s.ParentSkill).ThenInclude(s => s.ParentSkill)
                .SingleOrDefaultAsync(t => t.Id == message.TaskId);
        }
    }
}
