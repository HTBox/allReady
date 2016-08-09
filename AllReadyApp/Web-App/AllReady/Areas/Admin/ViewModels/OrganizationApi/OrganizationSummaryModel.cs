using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class OrganizationSummaryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Logo URL")]
        public string LogoUrl { get; set; }
        [Display(Name = "Website URL")]
        public string WebUrl { get; set; }
    }
}