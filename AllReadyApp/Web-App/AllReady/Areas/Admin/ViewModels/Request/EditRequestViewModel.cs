using System;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Request
{
    public class EditRequestViewModel
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int CampaignId { get; set; }

        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }
        public int EventId { get; set; }

        [Display(Name = "Event")]
        public string EventName { get; set; }

        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Mobile phone Number")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Unassigned;

        [Range(-90, 90, ErrorMessage = "Invalid Latitude.")]
        public double Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Invalid Longitude.")]
        public double Longitude { get; set; }

        public DateTime DateAdded { get; set; }

        public string Notes { get; set; }
    }
}
