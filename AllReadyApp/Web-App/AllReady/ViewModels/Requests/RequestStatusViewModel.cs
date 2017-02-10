using AllReady.Models;
using System;

namespace AllReady.ViewModels.Requests
{
    public class RequestStatusViewModel
    {

        public RequestStatus Status { get; set; }
        public bool HasItineraryItems { get; set; }
        public DateTime PlannedDeploymentDate { get; set; } //maps to Itinerary.Date

    }
}
