using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Site
{
    public class AssignOrganizationAdminModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int OrganizationId { get; set; }
    }
}