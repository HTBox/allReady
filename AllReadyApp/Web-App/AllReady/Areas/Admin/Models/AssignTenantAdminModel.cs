using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class AssignTenantAdminModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int TenantId { get; set; }
    }
}