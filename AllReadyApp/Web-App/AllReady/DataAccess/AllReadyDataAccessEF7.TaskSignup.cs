using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        IEnumerable<TaskSignup> IAllReadyDataAccess.TaskSignups
        {
            get
            {
                return _dbContext.TaskSignups
                          .Include(x => x.Task)
                          .Include(x => x.User)
                          .Include(x => x.Task.Activity)
                          .Include(x => x.Task.Activity.Campaign)
                          .Include(x => x.Task.Organization)
                          .ToArray();
            }
        }

        TaskSignup IAllReadyDataAccess.GetTaskSignup(int taskId, string userId)
        {
            return _dbContext.TaskSignups
              .Include(x => x.Task)
              .Include(x => x.User)
              .ToArray()
              .Where(x => x.Task.Id == taskId)
              .Where(x => x.User.Id.Equals(userId))
              .SingleOrDefault();
        }

        Task IAllReadyDataAccess.AddTaskSignupAsync(TaskSignup taskSignup)
        {
            _dbContext.TaskSignups.Add(taskSignup);
            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.DeleteTaskSignupAsync(int taskSignupId)
        {
            var taskSignup = _dbContext.TaskSignups.SingleOrDefault(c => c.Id == taskSignupId);

            if (taskSignup != null)
            {
                _dbContext.TaskSignups.Remove(taskSignup);
                return _dbContext.SaveChangesAsync();
            }
            return Task.FromResult(0);
        }

        Task IAllReadyDataAccess.UpdateTaskSignupAsync(TaskSignup value)
        {
            _dbContext.TaskSignups.Update(value);
            return _dbContext.SaveChangesAsync();
        }
    }
}
