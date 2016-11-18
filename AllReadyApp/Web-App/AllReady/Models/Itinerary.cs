using System;
using System.Collections.Generic;

namespace AllReady.Models
{
    /// <summary>
    /// Represents an itinerary for an event
    /// </summary>
    public class Itinerary
    {
        /// <summary>
        /// The itinerary id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The date on which the itinerary is scheduled to occur
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// A name to help identity the itinerary
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The id of the start location record
        /// </summary>
        public int? StartLocationId { get; set; }

        /// <summary>
        /// A navigation property to the location record for the start address
        /// </summary>
        public Location StartLocation { get; set; }

        /// <summary>
        /// The longitude of the start address
        /// </summary>
        public double StartLongitude { get; set; }

        /// <summary>
        /// The latitude of the start address
        /// </summary>
        public double StartLatitude { get; set; }

        /// <summary>
        /// The id of the end location record
        /// </summary>
        public int? EndLocationId { get; set; }

        /// <summary>
        /// A navigation property to the location record for the end address
        /// </summary>
        public Location EndLocation { get; set; }

        /// <summary>
        /// The longitude of the end address
        /// </summary>
        public double EndLongitude { get; set; }

        /// <summary>
        /// The latitude of the end address
        /// </summary>
        public double EndLatitude { get; set; }

        /// <summary>
        /// Indicates that the start address should be used for the end address also
        /// </summary>
        public bool UseStartAddressAsEndAddress { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public ICollection<ItineraryRequest> Requests { get; set; }
        public ICollection<TaskSignup> TeamMembers { get; set; }
    }
}
