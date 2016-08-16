using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Site
{
    public class AssignOrganizationAdminViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int OrganizationId { get; set; }
    }
}