using System;

namespace AllReady.Models
{
    public class VolunteerTaskSignup
    {
        public int Id { get; set; }

        public int VolunteerTaskId { get; set; }

        public VolunteerTask VolunteerTask { get; set; }

        public ApplicationUser User { get; set; }

        public string AdditionalInfo { get; set; }

        public DateTime StatusDateTimeUtc { get; set; } = DateTime.UtcNow;

        public VolunteerTaskStatus Status { get; set; }

        public string StatusDescription { get; set; }

        public int? ItineraryId { get; set; }

        public Itinerary Itinerary { get; set; }

        /// <summary>
        /// Indicates that this task signup represents the team leader
        /// </summary>
        public bool IsTeamLead { get; set; }
    }
}