using System;
using System.Collections.Generic;

namespace AllReady.Models
{
    // this is the class that represents incoming requests that 
    // ulitmately need to be mapped to volunteerTasks via a concept called
    // an itinerary. these could come from the AllReady app itself
    // or via a third party web request.

    public class Request
    {
        public Guid RequestId { get; set; }
        // allow for unique identifiers and mapping information
        public string ProviderRequestId { get; set; } //for RedCross, "serial"
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Unassigned;
        public RequestSource Source { get; set; } = RequestSource.Unknown;

        // no support yet for spatial types
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string ProviderData { get; set; } //for Red Cross, "assigned_rc_region"

        public int? EventId { get; set; }
        public Event Event { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public int? ItineraryId { get; set; }
        public ItineraryRequest Itinerary { get; set; }

        public string Notes { get; set; }

        /// <summary>
        /// Navigation property to get all comments for this request
        /// </summary>
        public virtual ICollection<RequestComment> RequestComments { get; set; }
    }
}
