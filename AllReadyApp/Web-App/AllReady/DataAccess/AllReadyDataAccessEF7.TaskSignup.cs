using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        IEnumerable<TaskUsers> IAllReadyDataAccess.TaskSignups
        {
            get
            {
                return _dbContext.TaskSignup
                          .Include(x => x.Task)
                          .Include(x => x.User)
                          .Include(x => x.Task.Activity)
                          .Include(x => x.Task.Activity.Campaign)
                          .Include(x => x.Task.Tenant)
                          .ToArray();
            }
        }

        TaskUsers IAllReadyDataAccess.GetTaskSignup(int taskId, string userId)
        {
            return _dbContext.TaskSignup
              .Include(x => x.Task)
              .Include(x => x.User)
              .ToArray()
              .Where(x => x.Task.Id == taskId)
              .Where(x => x.User.Id.Equals(userId))
              .SingleOrDefault();
        }

        Task IAllReadyDataAccess.AddTaskSignupAsync(TaskUsers taskSignup)
        {
            _dbContext.TaskSignup.Add(taskSignup);
            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.DeleteTaskSignupAsync(int taskSignupId)
        {
            var taskSignup = _dbContext.TaskSignup.SingleOrDefault(c => c.Id == taskSignupId);

            if (taskSignup != null)
            {
                _dbContext.TaskSignup.Remove(taskSignup);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IAllReadyDataAccess.UpdateTaskSignupAsync(TaskUsers value)
        {
            _dbContext.TaskSignup.Update(value);
            return _dbContext.SaveChangesAsync();
        }
    }
}
