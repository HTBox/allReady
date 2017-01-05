using System;

namespace AllReady.Models
{
    public class TaskSignup
    {
        public int Id { get; set; }

        public int TaskId { get; set; }

        public AllReadyTask Task { get; set; }

        public ApplicationUser User { get; set; }

        public string AdditionalInfo { get; set; }

        public DateTime StatusDateTimeUtc { get; set; } = DateTime.UtcNow;

        public TaskStatus Status { get; set; }

        public string StatusDescription { get; set; }

        public int? ItineraryId { get; set; }

        public Itinerary Itinerary { get; set; }
    }
}