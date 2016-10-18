using System;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Models
{
    /// <summary>
    /// Represents which requests are attached to which itinerary
    /// </summary>
    public class ItineraryRequest
    {
        [Required]
        public int ItineraryId { get; set; }
        public Itinerary Itinerary { get; set; }

        [Required]
        public Guid RequestId { get; set; }
        public Request Request { get; set; }

        //TODO: mgmccarthy: need to change this type from DateTime to DateTimeOffset in order to calculate local time of requestors to schedule the correct delivery time of sms confirmation messages for each request
        [Required]
        public DateTime DateAssigned { get; set; }

        [Required]
        public int OrderIndex { get; set; }
    }
}
