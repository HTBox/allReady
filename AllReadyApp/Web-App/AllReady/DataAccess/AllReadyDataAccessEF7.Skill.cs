using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        IEnumerable<Skill> IAllReadyDataAccess.Skills
        {
            get
            {
                return _dbContext.Skills
                    .Include(s => s.ParentSkill)
                    .Include(s => s.OwningOrganization)
                    .ToList();
            }
        }
    }
}
