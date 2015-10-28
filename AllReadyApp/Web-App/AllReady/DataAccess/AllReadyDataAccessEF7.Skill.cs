using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        IEnumerable<Skill> IAllReadyDataAccess.Skills
        {
            get
            {
                return _dbContext.Skills.Include(x => x.ParentSkill).ToList();
            }
        }
    }
}
