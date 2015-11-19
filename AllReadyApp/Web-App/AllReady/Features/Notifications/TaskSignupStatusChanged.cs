using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AllReady.Features.Notifications
{
    public class TaskSignupStatusChanged : INotification
    {
        public int SignupId { get; set; }
    }
}
