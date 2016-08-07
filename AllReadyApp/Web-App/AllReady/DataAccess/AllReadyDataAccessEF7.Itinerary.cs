using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        public IAsyncEnumerable<ItineraryRequest> ItineraryRequests => _dbContext.ItineraryRequests.AsAsyncEnumerable();

        public async Task<Itinerary> GetItineraryByIdAsync( int itineraryId )
        {
            return await _dbContext.Itineraries
              .Where(x => x.Id == itineraryId)
              .SingleOrDefaultAsync();
        }

        public async Task AddItineraryRequests(IEnumerable<ItineraryRequest> itineraryRequestsToAdd)
        {
            _dbContext.ItineraryRequests.AddRange(itineraryRequestsToAdd);
            await _dbContext.SaveChangesAsync();
        }
    }
}
