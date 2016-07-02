using System;
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
                    .ToList();
            }
        }

        EventSignup IAllReadyDataAccess.GetEventSignup(int eventId, string userId)
        {
            return _dbContext.EventSignup
                .Include(z => z.User)
                .Include(x => x.Event)
                .Where(x => x.Event.Id == eventId)
                .SingleOrDefault(x => x.User.Id == userId);
        }

        Task IAllReadyDataAccess.AddEventSignupAsync(EventSignup userSignup)
        {
            _dbContext.EventSignup.Add(userSignup);
            return _dbContext.SaveChangesAsync();
        }
    }
}
