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
                .ToArray()
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
            //TODO: Unvolunteer also remove Assigned Tasks
            var activity = _dbContext.ActivitySignup.SingleOrDefault(c => c.Id == activitySignupId);

            if (activity != null)
            {
                _dbContext.ActivitySignup.Remove(activity);
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
