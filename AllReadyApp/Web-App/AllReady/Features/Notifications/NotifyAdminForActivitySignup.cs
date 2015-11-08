using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

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
            throw new NotImplementedException();
        }
    }
}
