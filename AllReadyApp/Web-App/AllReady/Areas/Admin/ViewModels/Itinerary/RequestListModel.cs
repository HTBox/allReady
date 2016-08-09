using AllReady.Models;
using System;

namespace AllReady.Areas.Admin.Models.ItineraryModels
{
    public class RequestListModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DateAdded { get; set; }
        public RequestStatus Status { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
    }
}
