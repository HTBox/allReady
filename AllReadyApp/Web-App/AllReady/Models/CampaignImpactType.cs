using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
    /// <summary>
    /// Defines the type of campaign impact (numeric, text, etc.).
    /// Used to control the display of the impact as well as the
    /// edit form.
    /// </summary>
    public class CampaignImpactType
    {
        public int Id { get; set; }
        public string ImpactType { get; set; }
    }
}
