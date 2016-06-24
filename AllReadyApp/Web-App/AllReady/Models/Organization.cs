using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Models
{
    /// <summary>
    /// The highest level of organization in the application, typically represents a non-government organization (NGO)
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// The unique Id of the organization - controlled by SQL
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the organization
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URL of the logo used by the organization
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// The URL for the organization's website
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        /// Short summary description of the organzation which can be used in tiles and smaller display areas
        /// </summary>
        [MaxLength(250)]
        public string Summary { get; set; }

        /// <summary>
        /// Html content provided for the description of the organization
        /// </summary>
        public string DescriptionHtml { get; set; }

        /// <summary>
        /// Collection of the campaigns directly managed by this Organization
        /// </summary>
        public List<Campaign> Campaigns { get; set; }

        /// <summary>
        /// Application users which are members of this Organization.
        /// Users may be members of more than one organization.
        /// </summary>
        public List<ApplicationUser> Users { get; set; }

        /// <summary>
        /// The location object representing the location of the organization's office
        /// </summary>
        public Location Location { get; set; }

        public List<OrganizationContact> OrganizationContacts { get; set; }

        /// <summary>
        /// Represents html for an organization specific privacy policy
        /// </summary>
        public string PrivacyPolicy { get; set; }
    }
}
