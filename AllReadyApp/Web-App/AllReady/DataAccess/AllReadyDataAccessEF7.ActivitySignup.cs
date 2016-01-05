using System;
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

        ActivitySignup IAllReadyDataAccess.GetActivitySignup(int id, string userId)
        {
            return _dbContext.ActivitySignup
                .Include(z => z.User)
                .Include(x => x.Activity)
                .Include(x => x.Activity.UsersSignedUp)
                .Where(x => x.Activity.Id == id)
                .Where(x => x.User.Id == userId)
                .SingleOrDefault();
        }

        Task IAllReadyDataAccess.AddActivitySignupAsync(ActivitySignup userSignup)
        {
            _dbContext.ActivitySignup.Add(userSignup);
            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.DeleteActivitySignupAsync(int activitySignupId)
        {
            var activity = _dbContext.ActivitySignup.SingleOrDefault(c => c.Id == activitySignupId);

            if (activity != null)
            {
                _dbContext.ActivitySignup.Remove(activity);
                var signupIds = _dbContext.TaskSignups
                    .Where(e => e.Task.Activity.Id == activity.Activity.Id)
                    .Where(e => e.User.Id == activity.User.Id)
                    .Select(e => e.Id);

                var tasks = signupIds.Select(id => (this as IAllReadyDataAccess).DeleteTaskAsync(id));
                Task.WaitAll(tasks.ToArray());
                
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IAllReadyDataAccess.UpdateActivitySignupAsync(ActivitySignup value)
        {
            _dbContext.ActivitySignup.Update(value);
            return _dbContext.SaveChangesAsync();
        }
    }
}
