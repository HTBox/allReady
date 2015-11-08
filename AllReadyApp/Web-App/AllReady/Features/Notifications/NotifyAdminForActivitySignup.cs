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
        public NotifyAdminForActivitySignup(AllReadyContext context)
        {
            _context = context;
        }

        public void Handle(VolunteerInformationAdded notification)
        {
            var voluteer = _context.Users.Single(u => u.Id == notification.UserId);
            var activity = _context.Activities.Single(a => a.Id == notification.ActivityId);
            var campaign = _context.Campaigns
                .Include(c => c.Organizer)
                .Single(c => c.Id == activity.CampaignId);

            var message = $"A volunteer has signed up for {activity.Name}";


        }
    }
}
