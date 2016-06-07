using System;
using System.Collections.Generic;

namespace AllReady.Models
{
    /// <summary>
    /// Represents an itinerary for an event
    /// </summary>
    public class Itinerary
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public ICollection<ItineraryRequest> Requests { get; set; }
        public ICollection<TaskSignup> TeamMembers { get; set; }
    }
}
