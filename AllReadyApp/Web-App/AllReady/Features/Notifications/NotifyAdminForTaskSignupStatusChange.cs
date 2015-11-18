using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Features.Notifications
{
    public class NotifyAdminForTaskSignupStatusChange : INotificationHandler<TaskSignupStatusChanged>
    {
        public void Handle(TaskSignupStatusChanged notification)
        {
            // TODO: handle event
        }
    }
}
