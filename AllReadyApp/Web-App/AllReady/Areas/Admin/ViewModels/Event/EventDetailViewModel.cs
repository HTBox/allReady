﻿using System.Collections.Generic;
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
        /// A list of the skills required from volunteers of the event being displayed		
        /// </summary>
        [Display(Name = "Required Skills")]
        public List<string> RequiredSkillNames { get; set; } = new List<string>();

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

        /// <summary>
        /// The number of unassigned requests for this event
        /// </summary>
        public int UnassignedRequests { get; set; }

        /// <summary>
        /// The number of assigned requests for this event
        /// </summary>
        public int AssignedRequests { get; set; }

        public int PendingConfirmationRequests { get; set; }

        public int ConfirmedRequests { get; set; }

        /// <summary>
        /// The number of completed requests for this event
        /// </summary>
        public int CompletedRequests { get; set; }

        /// <summary>
        /// The number of canceled requests for this event
        /// </summary>
        public int CanceledRequests { get; set; }

        /// <summary>
        /// The calculated percentage of requests which are in the unassigned status for this event
        /// </summary>
        public string UnassignedPercentage
        {
            get
            {
                var percentage = 0.0;

                if (TotalRequests > 0)
                { 
                    percentage = UnassignedRequests / (double)TotalRequests * 100;
                }

                return percentage.ToString("0.0");
            }
        }

        /// <summary>
        /// The calculated percentage of requests which are in the assigned status for this event
        /// </summary>
        public string AssignedPercentage
        {
            get
            {
                var percentage = 0.0;

                if (TotalRequests > 0)
                {
                    percentage = (AssignedRequests/(double) TotalRequests)*100;
                }

                return percentage.ToString("0.0");
            }
        }

        public string PendingConfirmationPercentage
        {
            get
            {
                var percentage = 0.0;

                if (TotalRequests > 0)
                {
                    percentage = (PendingConfirmationRequests / (double)TotalRequests) * 100;
                }

                return percentage.ToString("0.0");
            }
        }

        public string ConfirmedPercentage
        {
            get
            {
                var percentage = 0.0;

                if (TotalRequests > 0)
                {
                    percentage = (ConfirmedRequests / (double)TotalRequests) * 100;
                }

                return percentage.ToString("0.0");
            }
        }

        /// <summary>
        /// The calculated percentage of requests which are in the completed status for this event
        /// </summary>
        public string CompletedPercentage
        {
            get
            {
                var percentage = 0.0;

                if (TotalRequests > 0)
                {
                    percentage = (CompletedRequests/(double) TotalRequests)*100;
                }

                return percentage.ToString("0.0");
            }
        }

        /// <summary>
        /// The calculated percentage of requests which are in the canceled status for this event
        /// </summary>
        public string CanceledPercentage
        {
            get
            {
                var percentage = 0.0;

                if (TotalRequests > 0)
                {
                    percentage = (CanceledRequests/(double) TotalRequests)*100;
                }

                return percentage.ToString("0.0");
            }
        }

        /// <summary>
        /// Model used to add a new itinerary
        /// </summary>
        public ItineraryEditViewModel NewItinerary { get; set; } = new ItineraryEditViewModel();

        /// <summary>
        /// The number of volunteers required across all tasks for the event
        /// </summary>
        public int VolunteersRequired { get; set; }

        /// <summary>
        /// The number of volunteers assigned to any of the event's tasks (in accepted status)
        /// </summary>
        public int AcceptedVolunteers { get; set; }

        /// <summary>
        /// The calculated percentage of volunteer fulfilment
        /// </summary>
        public string VolunteerFulfilmentPercentage
        {
            get
            {
                var percentage = 0.0;

                if (VolunteersRequired > 0)
                {
                    percentage = AcceptedVolunteers / (double)VolunteersRequired * 100;
                }

                return percentage.ToString("0.0");
            }
        }

        /// <summary>
        /// The number of tasks assigned to the event
        /// </summary>
        public int TaskCount => Tasks.Count;

        public string ItinerariesDetailsUrl { get; set; }
    }
}
