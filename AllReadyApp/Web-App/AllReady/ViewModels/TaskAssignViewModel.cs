﻿using System.Collections.Generic;

namespace AllReady.ViewModels
{
    public class TaskAssignViewModel
    {
        public int EventId { get; set; }
        public int VolunteerTaskId { get; set; }
        public List<string> AssignedUsers { get; set; }
    }
}
