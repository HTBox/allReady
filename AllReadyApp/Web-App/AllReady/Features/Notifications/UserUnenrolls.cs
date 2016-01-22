using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Notifications
{
    public class UserUnenrolls : INotification
    {
        public int ActivityId { get; set; }
        public string UserId { get; set; }
    }
}
