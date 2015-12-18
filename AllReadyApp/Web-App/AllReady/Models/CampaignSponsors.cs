using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
  public class CampaignSponsors
  {
    public int Id { get; set; }
    public Campaign Campaign { get; set; }
    public Organization Organization { get; set; }
  }
}
