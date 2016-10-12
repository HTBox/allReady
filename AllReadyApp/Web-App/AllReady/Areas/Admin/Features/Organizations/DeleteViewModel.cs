using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class DeleteViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Logo URL")]
        public string LogoUrl { get; set; }

        [Display(Name = "Website URL")]
        public string WebUrl { get; set; }

        public string Title { get; set; }
    }
}
