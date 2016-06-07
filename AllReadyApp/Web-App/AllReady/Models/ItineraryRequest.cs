using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
    /// <summary>
    /// Represents which requests are attached to which itinerary
    /// </summary>
    public class ItineraryRequest
    {
        public int ItineraryId { get; set; }
        public Itinerary Itinerary { get; set; }
        public Guid RequestId { get; set; }
        public Request Request { get; set; }
    }
}
