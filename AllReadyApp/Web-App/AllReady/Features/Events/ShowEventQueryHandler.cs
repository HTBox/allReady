using System.Collections.Generic;
using System.Security.Claims;
using AllReady.Models;
using AllReady.ViewModels.Event;
using AllReady.ViewModels.Shared;
using AllReady.ViewModels.Task;
using MediatR;
using System.Linq;

namespace AllReady.Features.Event
{
  public class ShowEventQueryHandler : IRequestHandler<ShowEventQuery, EventViewModel>
  {
    private readonly IAllReadyDataAccess _dataAccess;
    private SignInManager<ApplicationUser> _signInManager;
    private UserManager<ApplicationUser> _userManager;

    public ShowEventQueryHandler(IAllReadyDataAccess dataAccess, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
      _dataAccess = dataAccess;
      _userManager = userManager;
      _signInManager = signInManager;
    }

    public EventViewModel Handle(ShowEventQuery message)
    {
      var campaignEvent = _dataAccess.GetEvent(message.EventId);

      if (campaignEvent == null || campaignEvent.Campaign.Locked)
      {
        return null;
      }

            var eventViewModel = new EventViewModel(campaignEvent);

            var userId = message.User.GetUserId();
            var appUser = _dataAccess.GetUser(userId);
            eventViewModel.UserId = userId;
            eventViewModel.UserSkills = appUser?.AssociatedSkills?.Select(us => new SkillViewModel(us.Skill)).ToList();
            eventViewModel.IsUserVolunteeredForEvent = _dataAccess.GetEventSignups(eventViewModel.Id, userId).Any();

            var assignedTasks = campaignEvent.Tasks.Where(t => t.AssignedVolunteers.Any(au => au.User.Id == userId)).ToList();
            eventViewModel.UserTasks = new List<TaskViewModel>(assignedTasks.Select(data => new TaskViewModel(data, userId)).OrderBy(task => task.StartDateTime));

            var unassignedTasks = campaignEvent.Tasks.Where(t => t.AssignedVolunteers.All(au => au.User.Id != userId)).ToList();
            eventViewModel.Tasks = new List<TaskViewModel>(unassignedTasks.Select(data => new TaskViewModel(data, userId)).OrderBy(task => task.StartDateTime));
            eventViewModel.SignupModel = new EventSignupViewModel()
            {
                EventId = eventViewModel.Id,
                UserId = userId,
                Name = appUser.Name,
                PreferredEmail = appUser.Email,
                PreferredPhoneNumber = appUser.PhoneNumber
            };

            //return new EventViewModel(campaignEvent).WithUserInfo(campaignEvent, message.User, _dataAccess);
            return eventViewModel;
        }
    }
}
