using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AllReady.Features.Notifications
{
    public class VolunteerInformationAdded : IAsyncNotification
    {
        public int ActivityId { get; set; }
        public string UserId { get; set; }

    }
}
