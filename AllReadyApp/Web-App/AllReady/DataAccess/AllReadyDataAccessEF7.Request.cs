using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {

        public async Task<Request> GetRequestByProviderIdAsync( string providerId )
        {
            return await _dbContext.Requests.FirstOrDefaultAsync(x => providerId.Equals(x.ProviderId));
        }

        public async Task AddRequestAsync( Request request )
        {
            _dbContext.Requests.Add(request);

            await _dbContext.SaveChangesAsync();
        }
    }
}
