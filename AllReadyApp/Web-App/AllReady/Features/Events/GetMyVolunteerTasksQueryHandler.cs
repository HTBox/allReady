using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Event;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Events
{
    public class GetMyVolunteerTasksQueryHandler : IRequestHandler<GetMyVolunteerTasksQuery, IEnumerable<VolunteerTaskSignupViewModel>>
    {
        private readonly AllReadyContext dataContext;

        public GetMyVolunteerTasksQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public IEnumerable<VolunteerTaskSignupViewModel> Handle(GetMyVolunteerTasksQuery message)
        {
            var unfilteredTasks = this.dataContext.VolunteerTaskSignups
                .Include(ts => ts.VolunteerTask)
                .ThenInclude(t => t.Event)
                .ThenInclude(t => t.Campaign)
                .Include(ts => ts.User)
                .ToList();

            var finalTasks = unfilteredTasks.Where(ts => ts.VolunteerTask.Event.Id == message.EventId && ts.User.Id == message.UserId && !ts.VolunteerTask.Event.Campaign.Locked)
                .Select(t => new VolunteerTaskSignupViewModel(t))
                .ToList();

            return finalTasks;;
        }
    }
}
