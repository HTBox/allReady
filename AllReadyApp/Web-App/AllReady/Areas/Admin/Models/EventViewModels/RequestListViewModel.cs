using AllReady.Models;
using System;

namespace AllReady.Areas.Admin.Models.EventViewModels
{
    public class RequestListViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public DateTime DateAdded { get; set; }
        public RequestStatus Status { get; set; }

        public int ItineraryId { get; set; }
        public string ItineraryName { get; set; }
    }
}