using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;

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

      return new EventViewModel(campaignEvent)
          .WithUserInfo(campaignEvent, message.User, _dataAccess, _userManager, _signInManager);
    }
  }
}
