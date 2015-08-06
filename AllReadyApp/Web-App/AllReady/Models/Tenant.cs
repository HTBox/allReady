using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{

    /// <summary>
    /// The highest level of organization in the application, typically represents a non-government organization (NGO)
    /// </summary>
    public class Tenant
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string LogoUrl { get; set; }

        public string WebUrl { get; set; }

        /// <summary>
        /// Collection of the campaigns directly managed by this Tenant
        /// </summary>
        public List<Campaign> Campaigns { get; set; }

        /// <summary>
        /// Application users which are members of this Tenant.
        /// Users may be members of more than one tenant.
        /// </summary>
        public List<ApplicationUser> Users { get; set; }
    }
}
