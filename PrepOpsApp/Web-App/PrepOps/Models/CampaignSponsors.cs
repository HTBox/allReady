using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrepOps.Models
{
    public class CampaignSponsors
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Campaign Campaign { get; set; }

        public Tenant Tenant { get; set; }

    }
}
