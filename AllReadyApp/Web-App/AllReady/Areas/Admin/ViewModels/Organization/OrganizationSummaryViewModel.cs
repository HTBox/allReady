using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Organization
{
    public class OrganizationSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Logo URL")]
        public string LogoUrl { get; set; }
        [Display(Name = "Website URL")]
        public string WebUrl { get; set; }
    }
}