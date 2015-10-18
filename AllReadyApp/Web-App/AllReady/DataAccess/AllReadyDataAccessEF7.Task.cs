using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        IEnumerable<AllReadyTask> IAllReadyDataAccess.Tasks
        {
            get
            {
                return _dbContext.Tasks
                  .Include(x => x.Tenant)
                  .Include(x => x.Activity)
                  .Include(x => x.Activity.Campaign)
                  .Include(x => x.AssignedVolunteers)
                  .ToList();
            }
        }

        AllReadyTask IAllReadyDataAccess.GetTask(int taskId, string userId)
        {
            var taskUser = _dbContext.TaskSignup.Where(tu => tu.User.Id == userId).SingleOrDefault();

            return taskUser == null ? null :
                _dbContext.Tasks
                  .Include(x => x.Tenant)
                  .Include(x => x.Activity)
                  .Include(x => x.Activity.Campaign)
                  .Include(x => x.AssignedVolunteers)
                  .Where(t => t.Id == taskId && t.AssignedVolunteers.Contains(taskUser)).SingleOrDefault();
        }

        AllReadyTask IAllReadyDataAccess.GetTask(int taskId)
        {
            return _dbContext.Tasks
                .Include(x => x.Tenant)
                .Include(x => x.Activity)
                .Include(x => x.Activity.Campaign)
                .Include(x => x.AssignedVolunteers)
                .Where(t => t.Id == taskId).SingleOrDefault();
        }

        Task IAllReadyDataAccess.AddTaskAsync(AllReadyTask task)
        {
            if (task.Id == 0)
            {
                _dbContext.Add(task);
                return _dbContext.SaveChangesAsync();
            }
            else throw new InvalidOperationException("Added task that already has Id");
        }

        Task IAllReadyDataAccess.DeleteTaskAsync(int taskId)
        {
            var toDelete = _dbContext.Tasks.Where(t => t.Id == taskId).SingleOrDefault();

            if (toDelete != null)
            {
                _dbContext.Tasks.Remove(toDelete);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IAllReadyDataAccess.UpdateTaskAsync(AllReadyTask value)
        {
            _dbContext.Tasks.Update(value);
            return _dbContext.SaveChangesAsync();
        }
    }
}
