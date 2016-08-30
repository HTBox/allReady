using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Event
{
    /// <summary>
    /// Defines data used by the admin event details page
    /// </summary>
    public class EventDetailViewModel : EventSummaryViewModel
    {
        /// <summary>
        /// The location of the event being displayed
        /// </summary>
        [UIHint("Location")]
        public LocationEditViewModel Location { get; set; }

        /// <summary>
        /// A list of the tasks currently associated with the event being displayed
        /// </summary>
        public IList<TaskSummaryViewModel> Tasks { get; set; } = new List<TaskSummaryViewModel>();

        /// <summary>
        /// A list of the volunteers currently registered for the event being displayed
        /// </summary>
        public IList<string> Volunteers { get; set; } = new List<string>();

        /// <summary>
        /// A list of the skills required from volunteers of the event being displayed
        /// </summary>
        [Display(Name = "Required Skills")]
        public IEnumerable<EventSkill> RequiredSkills { get; set; } = new List<EventSkill>();

        /// <summary>
        /// An enumerable of itineraries associated with the event being displayed
        /// </summary>
        public IEnumerable<ItineraryListViewModel> Itineraries { get; set; } = new List<ItineraryListViewModel>();

        /// <summary>
        /// Indicates whether the event is an itinerary event
        /// </summary>
        public bool IsItineraryEvent => EventType == EventType.Itinerary;

        /// <summary>
        /// The total number of requests associated with the event being displayed
        /// </summary>
        public int TotalRequests { get; set; }

        public int UnassignedRequests { get; set; }

        public int AssignedRequests { get; set; }

        public int CompletedRequests { get; set; }

        public int CanceledRequests { get; set; }

        public string UnassignedPercentage
        {
            get
            {
                var percentage = 0.0;

                if (TotalRequests > 0)
                { 
                    percentage = ((double)UnassignedRequests / (double)TotalRequests) * 100;
                }

                return percentage.ToString("0.0");
            }
        }

        public string AssignedPercentage
        {
            get
            {
                var percentage = 0.0;

                if (TotalRequests > 0)
                {
                    percentage = ((double) AssignedRequests/(double) TotalRequests)*100;
                }

                return percentage.ToString("0.0");
            }
        }

        public string CompletedPercentage
        {
            get
            {
                var percentage = 0.0;

                if (TotalRequests > 0)
                {
                    percentage = ((double) CompletedRequests/(double) TotalRequests)*100;
                }

                return percentage.ToString("0.0");
            }
        }

        public string CanceledPercentage
        {
            get
            {
                var percentage = 0.0;

                if (TotalRequests > 0)
                {
                    percentage = ((double) CanceledRequests/(double) TotalRequests)*100;
                }

                return percentage.ToString("0.0");
            }
        }

        /// <summary>
        /// Model used to add a new itinerary
        /// </summary>
        public ItineraryEditViewModel NewItinerary { get; set; } = new ItineraryEditViewModel();

        public string ItinerariesDetailsUrl { get; set; }
    }
}
