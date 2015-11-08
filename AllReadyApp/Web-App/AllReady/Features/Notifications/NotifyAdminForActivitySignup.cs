using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForActivitySignup: INotificationHandler<VolunteerInformationAdded>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _bus;

        public NotifyAdminForActivitySignup(AllReadyContext context, IMediator bus)
        {
            _context = context;
            _bus = bus;
        }

        public void Handle(VolunteerInformationAdded notification)
        {
            var voluteer = _context.Users.Single(u => u.Id == notification.UserId);
            var activity = _context.Activities.Single(a => a.Id == notification.ActivityId);
            var campaign = _context.Campaigns
                .Include(c => c.Organizer)
                .Single(c => c.Id == activity.CampaignId);

            var subject = $"A volunteer has signed up for {activity.Name}";

            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    EmailMessage = $"Your {campaign.Name} campaign activity '{activity.Name}' has a new volunteer. {voluteer.UserName} can be reached at {voluteer.Email}.",
                    EmailRecipients = new List<string> { campaign.Organizer.Email },
                    Subject = subject
                }
            };

            _bus.Send(command);
        }
    }
}
