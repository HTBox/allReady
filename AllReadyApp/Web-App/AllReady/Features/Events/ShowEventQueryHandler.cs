using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels.Event;
using AllReady.ViewModels.Task;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VolunteerTaskSignupViewModel = AllReady.ViewModels.Shared.VolunteerTaskSignupViewModel;

namespace AllReady.Features.Events
{
    public class ShowEventQueryHandler : IAsyncRequestHandler<ShowEventQuery, EventViewModel>
    {
        private readonly AllReadyContext _context;

        public ShowEventQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EventViewModel> Handle(ShowEventQuery message)
        {
            var @event = await _context.Events
                .Include(a => a.Location)
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.RequiredSkills).ThenInclude(rs => rs.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.VolunteerTasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(tu => tu.User)
                .Include(a => a.VolunteerTasks).ThenInclude(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .SingleOrDefaultAsync(a => a.Id == message.EventId);

            if (@event == null || @event.Campaign.Locked)
            {
                return null;
            }

            var eventViewModel = new EventViewModel(@event);

            ApplicationUser applicationUser = null;

            if (!string.IsNullOrEmpty(message.UserId))
            {
                applicationUser = await _context.Users.SingleOrDefaultAsync(u => u.Id == message.UserId);

                eventViewModel.UserId = message.UserId;
                eventViewModel.UserSkills = applicationUser?.AssociatedSkills?.Select(us => new SkillViewModel(us.Skill)).ToList();

                var assignedTasks = @event.VolunteerTasks.Where(t => t.AssignedVolunteers.Any(au => au.User.Id == message.UserId)).ToList();
                eventViewModel.UserTasks = new List<VolunteerTaskViewModel>(assignedTasks.Select(data => new VolunteerTaskViewModel(data, message.UserId)).OrderBy(task => task.StartDateTime));

                var unassignedTasks = @event.VolunteerTasks.Where(t => t.AssignedVolunteers.All(au => au.User.Id != message.UserId)).ToList();
                eventViewModel.Tasks = new List<VolunteerTaskViewModel>(unassignedTasks.Select(data => new VolunteerTaskViewModel(data, message.UserId)).OrderBy(task => task.StartDateTime));
            }

            eventViewModel.SignupModel = new VolunteerTaskSignupViewModel
            {
                EventId = eventViewModel.Id,
                UserId = message.UserId,
                Name = applicationUser?.Name
            };

            return eventViewModel;
        }
    }
}
