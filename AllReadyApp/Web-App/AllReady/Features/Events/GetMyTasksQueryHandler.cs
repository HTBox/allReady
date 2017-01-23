﻿using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Event;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Events
{
    public class GetMyTasksQueryHandler : IRequestHandler<GetMyTasksQuery, IEnumerable<TaskSignupViewModel>>
    {
        private readonly AllReadyContext dataContext;

        public GetMyTasksQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public IEnumerable<TaskSignupViewModel> Handle(GetMyTasksQuery message)
        {
            var unfilteredTasks = this.dataContext.TaskSignups
                .Include(ts => ts.VolunteerTask)
                .ThenInclude(t => t.Event)
                .ThenInclude(t => t.Campaign)
                .Include(ts => ts.User)
                .ToList();

            var finalTasks = unfilteredTasks.Where(ts => ts.VolunteerTask.Event.Id == message.EventId && ts.User.Id == message.UserId && !ts.VolunteerTask.Event.Campaign.Locked)
                .Select(t => new TaskSignupViewModel(t))
                .ToList();

            return finalTasks;;
        }
    }
}
