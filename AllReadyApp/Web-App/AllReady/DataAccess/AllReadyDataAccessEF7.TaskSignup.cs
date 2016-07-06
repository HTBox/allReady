using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
                          .Include(x => x.Task.Event)
                          .Include(x => x.Task.Event.Campaign)
                          .Include(x => x.Task.Organization)
                          .ToArray();
            }
        }

        Task IAllReadyDataAccess.UpdateTaskSignupAsync(TaskSignup value)
        {
            _dbContext.TaskSignups.Update(value);
            return _dbContext.SaveChangesAsync();
        }
    }
}
