﻿using MediatR;

namespace AllReady.Features.Notifications
{
    public class VolunteerSignupNotification : INotification, IAsyncNotification
    {
        public int EventId { get; set; }
        public int? TaskId { get; set; }
        public string UserId { get; set; }

    }
}
