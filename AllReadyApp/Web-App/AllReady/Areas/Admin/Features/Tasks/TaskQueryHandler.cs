using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var task = _context.Tasks
                .AsNoTracking()
                .Include(t => t.Activity).ThenInclude(a => a.UsersSignedUp).ThenInclude(us => us.User)
                .Include(t=> t.Activity.Campaign)
                .Include(t => t.AssignedVolunteers).ThenInclude(av => av.User)

                .SingleOrDefault(t => t.Id == message.TaskId);
            var taskModel = new TaskSummaryModel()
            {
                Id = task.Id,
                ActivityId = task.Activity.Id,
                ActivityName = task.Activity.Name,
                CampaignId = task.Activity.CampaignId,
                CampaignName = task.Activity.Campaign.Name,
                TenantId = task.Activity.TenantId,
                Name = task.Name,
                Description = task.Description,
                StartDateTime = task.StartDateTimeUtc,
                EndDateTime = task.EndDateTimeUtc,
                NumberOfVolunteersRequired = task.NumberOfVolunteersRequired,
                AssignedVolunteers = task.AssignedVolunteers.Select(av => new VolunteerModel { UserId = av.User.Id, UserName = av.User.UserName, HasVolunteered = true }).ToList(),
                AllVolunteers = task.Activity.UsersSignedUp.Select(v => new VolunteerModel { UserId = v.User.Id, UserName = v.User.UserName, HasVolunteered = false }).ToList()
            };
            foreach (var av in taskModel.AssignedVolunteers)
                {
                var v = taskModel.AllVolunteers.Single(al => al.UserId == av.UserId);
                v.HasVolunteered = true;
            }
            return taskModel;
        }
    }
}
