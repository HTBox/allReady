using System;
using AllReady.Areas.Admin.ViewModels.Shared;

namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    /// <summary>
    /// Defines summary information for an itinerary
    /// </summary>
    public class ItinerarySummaryViewModel
    {
        /// <summary>
        /// The ID of the itinerary being displayed
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the itinerary being displayed
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The date on which the itinerary will take place
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The ID of the Organization who owns the itinerary
        /// </summary>
        public int OrganizationId { get; set; }

        /// <summary>
        /// Summary information for the event. By default this only loads the id and the start and end dates
        /// </summary>
        public EventSummaryViewModel EventSummary { get; set; }

        /// <summary>
        /// The date on which the itinerary will take place (in date long string format)
        /// </summary>
        public string DisplayDate => Date.ToString("D");
    }
}
