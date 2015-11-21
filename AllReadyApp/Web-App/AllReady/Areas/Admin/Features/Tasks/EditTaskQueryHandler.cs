﻿using AllReady.Areas.Admin.Models;
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
                    TenantId = task.Activity.Campaign.ManagingTenantId,
                    Name = task.Name,
                    Description = task.Description,
                    StartDateTime = task.StartDateTimeUtc,
                    EndDateTime = task.EndDateTimeUtc,
                    NumberOfVolunteersRequired = task.NumberOfVolunteersRequired,
                    RequiredSkills = task.RequiredSkills
                };
                    
            return viewModel;
        }
    }
}
