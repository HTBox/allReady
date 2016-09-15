using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels.Event;
using AllReady.ViewModels.Task;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskSignupViewModel = AllReady.ViewModels.Shared.TaskSignupViewModel;

namespace AllReady.Features.Events
{
    public class ShowEventQueryHandlerAsync : IAsyncRequestHandler<ShowEventQueryAsync, EventViewModel>
    {
        private readonly AllReadyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShowEventQueryHandlerAsync(AllReadyContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<EventViewModel> Handle(ShowEventQueryAsync message)
        {
            var @event = await _context.Events
                .Include(a => a.Location)
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.RequiredSkills).ThenInclude(rs => rs.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(tu => tu.User)
                .Include(a => a.Tasks).ThenInclude(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .SingleOrDefaultAsync(a => a.Id == message.EventId)
                .ConfigureAwait(false);

            if (@event == null || @event.Campaign.Locked)
            {
                return null;
            }

            var eventViewModel = new EventViewModel(@event);

            var user = await _userManager.GetUserAsync(message.User).ConfigureAwait(false);
            var userId = user.Id;
            var appUser = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);

            eventViewModel.UserId = userId;
            eventViewModel.UserSkills = appUser?.AssociatedSkills?.Select(us => new SkillViewModel(us.Skill)).ToList();

            var assignedTasks = @event.Tasks.Where(t => t.AssignedVolunteers.Any(au => au.User.Id == userId)).ToList();
            eventViewModel.UserTasks = new List<TaskViewModel>(assignedTasks.Select(data => new TaskViewModel(data, userId)).OrderBy(task => task.StartDateTime));

            var unassignedTasks = @event.Tasks.Where(t => t.AssignedVolunteers.All(au => au.User.Id != userId)).ToList();
            eventViewModel.Tasks = new List<TaskViewModel>(unassignedTasks.Select(data => new TaskViewModel(data, userId)).OrderBy(task => task.StartDateTime));

            eventViewModel.SignupModel = new TaskSignupViewModel
            {
                EventId = eventViewModel.Id,
                UserId = userId,
                Name = appUser?.Name
            };

            return eventViewModel;
        }
    }
}