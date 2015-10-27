using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace AllReady.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
  {
        public List<Skill> AssociatedSkills { get; set; }

  }
}
