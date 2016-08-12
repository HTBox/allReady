﻿using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskQueryHandlerAsync : IAsyncRequestHandler<EditTaskQueryAsync, TaskSummaryModel>
    {
        private AllReadyContext _context;

        public EditTaskQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<TaskSummaryModel> Handle(EditTaskQueryAsync message)
        {
            var task = await _context.Tasks.AsNoTracking()
                .Include(t => t.Event).ThenInclude(a => a.Campaign)
                .Include(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .SingleOrDefaultAsync(t => t.Id == message.TaskId);

            var viewModel = new TaskSummaryModel
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
                RequiredSkills = task.RequiredSkills
            };
                    
            return viewModel;
        }
    }
}
