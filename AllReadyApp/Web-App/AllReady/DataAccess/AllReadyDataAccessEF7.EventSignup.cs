using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

    }
}
