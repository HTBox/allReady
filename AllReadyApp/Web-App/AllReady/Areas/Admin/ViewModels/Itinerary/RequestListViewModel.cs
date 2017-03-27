using System;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    public class RequestListViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DateAdded { get; set; }
        public RequestStatus Status { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }

        public int ItineraryId { get; set; }
        public string ItineraryName { get; set; }
    }
}
