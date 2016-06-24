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
        public DateTime DateAdded { get; set; }
        public RequestStatus Status { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
    }
}
