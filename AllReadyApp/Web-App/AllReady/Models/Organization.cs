using System.Collections.Generic;
namespace AllReady.Models
{
  /// <summary>
  /// The highest level of organization in the application, typically represents a non-government organization (NGO)
  /// </summary>
  public class Organization
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string LogoUrl { get; set; }
    public string WebUrl { get; set; }

        /// <summary>
        /// Collection of the campaigns directly managed by this Organization
        /// </summary>
        public List<Campaign> Campaigns { get; set; }

        /// <summary>
        /// Application users which are members of this Organization.
        /// Users may be members of more than one organization.
        /// </summary>
        public List<ApplicationUser> Users { get; set; }

        public Location Location { get; set; }

        public List<OrganizationContact> OrganizationContacts { get; set; }
  }
}
