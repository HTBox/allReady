using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
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
                .Select(t => new TaskSummaryModel()
                {
                    Id = t.Id,
                    ActivityId = t.Activity.Id,
                    ActivityName = t.Activity.Name,
                    CampaignId = t.Activity.CampaignId,
                    CampaignName = t.Activity.Campaign.Name,
                    TenantId = t.Activity.TenantId,
                    Name = t.Name,
                    Description = t.Description,
                    StartDateTime = t.StartDateTimeUtc,
                    EndDateTime = t.EndDateTimeUtc,
                    NumberOfVolunteersRequired = t.NumberOfVolunteersRequired
                }).SingleOrDefault(t => t.Id == message.TaskId);
                    
            return task;
        }
    }
}
