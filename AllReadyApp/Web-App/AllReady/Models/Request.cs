using System;

namespace AllReady.Models
{
    // this is the class that represents incoming requests that 
    // ulitmately need to be mapped to tasks via a concept called
    // an itinerary. these could come from the AllReady app itself
    // or via a third party web request.

    public class Request
    {
        // basic data
        public Guid RequestId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Unassigned;

        /// <summary>
        /// The source of the request
        /// </summary>
        public RequestSource Source { get; set; } = RequestSource.Unknown;

        // no support yet for spatial types
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // allow for unique identifiers and mapping information
        public string ProviderId { get; set; }      // for RedCross, "serial"

        //TODO: mgmccarthy. We might not have to store this in the Request table
        public string ProviderData { get; set; }    // for Red Cross, "assigned_rc_region"

        public int? EventId { get; set; }
        public Event Event { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}