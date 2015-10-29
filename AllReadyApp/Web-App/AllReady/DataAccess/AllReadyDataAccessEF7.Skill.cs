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
                return _dbContext.Skills.ToList();
            }
        }

        Skill IAllReadyDataAccess.GetSkill(int skillId)
        {
            return _dbContext.Skills.ToList().SingleOrDefault(x => x.Id == skillId);
        }

        Task IAllReadyDataAccess.AddSkill(Skill value)
        {
            _dbContext.Skills.Add(value);
            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.DeleteSkill(int skillId)
        {
            var toDelete = _dbContext.Skills.Where(c => c.Id == skillId).SingleOrDefault();

            if (toDelete != null)
            {
                _dbContext.Skills.Remove(toDelete);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IAllReadyDataAccess.UpdateSkill(Skill value)
        {
            _dbContext.Skills.Update(value);
            return _dbContext.SaveChangesAsync();
        }
    }
}
