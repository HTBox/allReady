using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class AssignOrganizationAdminModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int OrganizationId { get; set; }
    }
}