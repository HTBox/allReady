﻿using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Campaign
{
    public class PublishViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int OrganizationId { get; set; }

        [Display(Name = "Organization")]
        public string OrganizationName { get; set; }

        public string Title { get; set; }

        //used to carry value from GET to POST action method in controller, not stored in a model anywhere
        public bool UserIsOrgAdmin { get; set; }
    }
}
