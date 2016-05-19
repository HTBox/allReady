using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        IEnumerable<ApplicationUser> IAllReadyDataAccess.Users
        {
            get
            {
                return _dbContext.Users
                    .Include(u => u.AssociatedSkills)
                    .Include(u => u.Claims)
                    .ToList();
            }
        }

        ApplicationUser IAllReadyDataAccess.GetUser(string userId)
        {
            return _dbContext.Users
                .Where(u => u.Id == userId)
                .Include(u => u.AssociatedSkills).ThenInclude((UserSkill us) => us.Skill)
                .Include(u => u.Claims)
                .SingleOrDefault();
        }

        Task IAllReadyDataAccess.UpdateUser(ApplicationUser value)
        {
            //First remove any skills that are no longer associated with this user
            var usksToRemove = _dbContext.UserSkills.Where(usk => usk.UserId == value.Id && (value.AssociatedSkills == null ||
                !value.AssociatedSkills.Any(usk1 => usk1.SkillId == usk.SkillId)));
            _dbContext.UserSkills.RemoveRange(usksToRemove);
            _dbContext.Users.Update(value);
            return _dbContext.SaveChangesAsync();
        }
    }
}
