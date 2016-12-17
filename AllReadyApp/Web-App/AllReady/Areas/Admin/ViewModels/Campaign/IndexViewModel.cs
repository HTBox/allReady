using System;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Campaign
{
    public class IndexViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Organization")]
        public string OrganizationName { get; set; }

        public bool Featured { get; set; }

        public bool Locked { get; set; }
        public bool Published { get; set; }

        public string Description { get; set; }

        [Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTimeOffset StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTimeOffset EndDate { get; set; }
    }
}
