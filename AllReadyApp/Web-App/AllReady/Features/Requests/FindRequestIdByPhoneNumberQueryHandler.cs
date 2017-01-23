using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Features.Requests
{
    public class FindRequestIdByPhoneNumberQueryHandler : IAsyncRequestHandler<FindRequestIdByPhoneNumberQuery, Guid>
    {
        private readonly AllReadyContext _context;

        public FindRequestIdByPhoneNumberQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(FindRequestIdByPhoneNumberQuery message)
        {
            // note - stevejgordon - for now we are taking the most recent matching request - later this may be enhanced to use the SMS log table

            return await _context.Requests.AsNoTracking()
                .Where(rec => rec.Phone == message.PhoneNumber)
                .OrderByDescending(rec => rec.DateAdded)
                .Select(rec => rec.RequestId)
                .FirstOrDefaultAsync();
        }
    }
}
