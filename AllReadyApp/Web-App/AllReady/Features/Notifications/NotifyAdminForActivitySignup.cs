using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForActivitySignu: INotificationHandler<VolunteerInformationAdded>
    {
        public void Handle(VolunteerInformationAdded notification)
        {
            throw new NotImplementedException();
        }
    }
}
