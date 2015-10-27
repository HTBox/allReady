using System;
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
                return _dbContext.Users.ToList();
            }
        }
        ApplicationUser IAllReadyDataAccess.GetUser(string userId)
        {
            return _dbContext.Users.Where(u => u.Id == userId).SingleOrDefault();
        }

        Task IAllReadyDataAccess.AddUser(ApplicationUser value)
        {
            _dbContext.Users.Add(value);
            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.DeleteUser(string userId)
        {
            var toDelete = _dbContext.Users.Where(u => u.Id == userId).SingleOrDefault();
            if (toDelete != null)
            {
                _dbContext.Users.Remove(toDelete);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IAllReadyDataAccess.UpdateUser(ApplicationUser value)
        {
            _dbContext.Users.Update(value);
            return _dbContext.SaveChangesAsync();
        }
    }
}
