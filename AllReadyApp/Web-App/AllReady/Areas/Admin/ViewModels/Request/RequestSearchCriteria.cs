﻿using System;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Request
{
    public class RequestSearchCriteria
    {
        public Guid? RequestId { get; set; }
        public bool IncludeAssigned { get; set; } = false;
        public bool IncludeCanceled { get; set; } = false;
        public int? EventId { get; set; }
        public int? ItineraryId { get; set; }

        public string Keywords { get; set; }
        public RequestStatus? Status { get; set; }
        public int? OrganizationId { get; set; }
    }
}
