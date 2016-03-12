using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class TaskQueryHandler : IRequestHandler<TaskQuery, TaskSummaryModel>
    {
        private AllReadyContext _context;

        public TaskQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public TaskSummaryModel Handle(TaskQuery message)
        {
            var task = GetTask(message);

            var taskModel = new TaskSummaryModel()
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
                AssignedVolunteers = task.AssignedVolunteers.Select(av => new VolunteerModel { UserId = av.User.Id, UserName = av.User.UserName, HasVolunteered = true }).ToList(),
                AllVolunteers = task.Activity.UsersSignedUp.Select(v => new VolunteerModel { UserId = v.User.Id, UserName = v.User.UserName, HasVolunteered = false }).ToList()
            };

            foreach (var assignedVolunteer in taskModel.AssignedVolunteers)
            {
                var v = taskModel.AllVolunteers.Single(al => al.UserId == assignedVolunteer.UserId);
                v.HasVolunteered = true;
            }
            return taskModel;
        }

        private AllReadyTask GetTask(TaskQuery message)
        {
            return _context.Tasks
                .AsNoTracking()
                .Include(t => t.Activity).ThenInclude(a => a.UsersSignedUp).ThenInclude(us => us.User)
                .Include(t => t.Activity.Campaign)
                .Include(t => t.AssignedVolunteers).ThenInclude(av => av.User)
                .SingleOrDefault(t => t.Id == message.TaskId);
        }
    }
}
