using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Campaign
{
    public class DeleteViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int OrganizationId { get; set; }

        [Display(Name = "Organization")]
        public string OrganizationName { get; set; }

        public string Title { get; set; }
    }
}
