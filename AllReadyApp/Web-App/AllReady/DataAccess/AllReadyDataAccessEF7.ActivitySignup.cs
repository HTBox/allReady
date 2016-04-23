using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        IEnumerable<EventSignup> IAllReadyDataAccess.EventSignups
        {
            get
            {
                return _dbContext.EventSignup
                    .Include(z => z.User)
                    .Include(x => x.Event)
                    .Include(x => x.Event.UsersSignedUp)
                    .ThenInclude(u => u.User)
                    .ToList();
            }
        }

        EventSignup IAllReadyDataAccess.GetEventSignup(int eventId, string userId)
        {
            return _dbContext.EventSignup
                .Include(z => z.User)
                .Include(x => x.Event)
                .Include(x => x.Event.UsersSignedUp)
                .Where(x => x.Event.Id == eventId)
                .SingleOrDefault(x => x.User.Id == userId);
        }

        Task IAllReadyDataAccess.AddEventSignupAsync(EventSignup userSignup)
        {
            _dbContext.EventSignup.Add(userSignup);
            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.DeleteEventAndTaskSignupsAsync(int eventSignupId)
        {
            var eventSignup = _dbContext.EventSignup.SingleOrDefault(c => c.Id == eventSignupId);

            if (eventSignup == null)
            {
                return Task.FromResult(0);
            }
            
            _dbContext.EventSignup.Remove(eventSignup);

            _dbContext.TaskSignups.RemoveRange(_dbContext.TaskSignups
                .Where(e => e.Task.Event.Id == eventSignup.Event.Id)
                .Where(e => e.User.Id == eventSignup.User.Id));
                
            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.UpdateEventSignupAsync(EventSignup value)
        {
            _dbContext.EventSignup.Update(value);
            return _dbContext.SaveChangesAsync();
        }
    }
}
