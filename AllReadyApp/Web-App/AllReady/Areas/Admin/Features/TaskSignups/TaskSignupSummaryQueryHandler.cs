﻿using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.TaskSignups
{
    public class TaskSignupSummaryQueryHandler : IAsyncRequestHandler<TaskSignupSummaryQuery, TaskSignupSummaryViewModel>
    {
        private readonly AllReadyContext _context;

        public TaskSignupSummaryQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<TaskSignupSummaryViewModel> Handle(TaskSignupSummaryQuery message)
        {
            return await _context.VolunteerTaskSignups.AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.Id == message.VolunteerTaskSignupId)
                .Select( x =>
                new TaskSignupSummaryViewModel
                {
                    VolunteerTaskSignupId = x.Id,
                    VolunteerName = x.User.Name,
                    VolunteerEmail = x.User.Email
                })
                .FirstOrDefaultAsync();
        }
    }
}