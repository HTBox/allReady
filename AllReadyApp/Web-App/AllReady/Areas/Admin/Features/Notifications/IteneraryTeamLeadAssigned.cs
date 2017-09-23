using System;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class IteneraryTeamLeadAssigned : IAsyncNotification
    {
        public string AssigneeEmail { get; set; }
        public string ItineraryName { get; set; }
        public string AssigneePhone { get; set; }
        public string IteneraryUrl { get; set; }
    }

}
