using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        IEnumerable<ActivitySignup> IAllReadyDataAccess.ActivitySignups
        {
            get
            {
                return _dbContext.ActivitySignup
                    .Include(z => z.User)
                    .Include(x => x.Activity)
                    .Include(x => x.Activity.UsersSignedUp)
                    .ThenInclude(u => u.User)
                    .ToList();
            }
        }

        ActivitySignup IAllReadyDataAccess.GetActivitySignup(int activityId, string userId)
        {
            return _dbContext.ActivitySignup
                .Include(z => z.User)
                .Include(x => x.Activity)
                .Include(x => x.Activity.UsersSignedUp)
                .Where(x => x.Activity.Id == activityId)
                .SingleOrDefault(x => x.User.Id == userId);
        }

        Task IAllReadyDataAccess.AddActivitySignupAsync(ActivitySignup userSignup)
        {
            _dbContext.ActivitySignup.Add(userSignup);
            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.DeleteActivityAndTaskSignupsAsync(int activitySignupId)
        {
            var activitySignup = _dbContext.ActivitySignup.SingleOrDefault(c => c.Id == activitySignupId);

            if (activitySignup == null)
                return Task.FromResult(0);

            _dbContext.ActivitySignup.Remove(activitySignup);

            _dbContext.TaskSignups.RemoveRange(_dbContext.TaskSignups
                .Where(e => e.Task.Activity.Id == activitySignup.Activity.Id)
                .Where(e => e.User.Id == activitySignup.User.Id));

            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.UpdateActivitySignupAsync(ActivitySignup value)
        {
            _dbContext.ActivitySignup.Update(value);
            return _dbContext.SaveChangesAsync();
        }
    }
}
