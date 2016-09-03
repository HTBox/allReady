using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Event;
using AllReady.ViewModels.Shared;
using AllReady.ViewModels.Task;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskSignupViewModel = AllReady.ViewModels.Shared.TaskSignupViewModel;

namespace AllReady.Features.Events
{
    public class ShowEventQueryHandler : IRequestHandler<ShowEventQuery, EventViewModel>
    {
        private readonly AllReadyContext dataContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShowEventQueryHandler(AllReadyContext dataContext, UserManager<ApplicationUser> userManager)
        {
            this.dataContext = dataContext;
            _userManager = userManager;
        }

        public EventViewModel Handle(ShowEventQuery message)
        {
            int eventId = message.EventId;

            // TODO: do we need all these includes?
            var evt = this.dataContext.Events
                .Include(a => a.Location)
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.RequiredSkills).ThenInclude(rs => rs.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(tu => tu.User)
                .Include(a => a.Tasks).ThenInclude(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .SingleOrDefault(a => a.Id == eventId);

            if (evt == null || evt.Campaign.Locked)
            {
                return null;
            }

            var eventViewModel = new EventViewModel(evt);

            var userId = _userManager.GetUserId(message.User);
            var appUser = this.dataContext.Users.SingleOrDefault(u => u.Id == userId);

            eventViewModel.UserId = userId;
            eventViewModel.UserSkills = appUser?.AssociatedSkills?.Select(us => new SkillViewModel(us.Skill)).ToList();

            var assignedTasks = evt.Tasks.Where(t => t.AssignedVolunteers.Any(au => au.User.Id == userId)).ToList();
            eventViewModel.UserTasks = new List<TaskViewModel>(assignedTasks.Select(data => new TaskViewModel(data, userId)).OrderBy(task => task.StartDateTime));

            var unassignedTasks = evt.Tasks.Where(t => t.AssignedVolunteers.All(au => au.User.Id != userId)).ToList();
            eventViewModel.Tasks = new List<TaskViewModel>(unassignedTasks.Select(data => new TaskViewModel(data, userId)).OrderBy(task => task.StartDateTime));

            eventViewModel.SignupModel = new TaskSignupViewModel
            {
                EventId = eventViewModel.Id,
                UserId = userId,
                Name = appUser?.Name,
                PreferredEmail = appUser?.Email,
                PreferredPhoneNumber = appUser?.PhoneNumber
            };

            return eventViewModel;
        }
    }
}
