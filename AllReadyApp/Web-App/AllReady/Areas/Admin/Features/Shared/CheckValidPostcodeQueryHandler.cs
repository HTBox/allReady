using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Shared
{
    public class CheckValidPostcodeQueryHandler : IAsyncRequestHandler<CheckValidPostcodeQuery, bool>
    {
        private AllReadyContext _context;
        public CheckValidPostcodeQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CheckValidPostcodeQuery message)
        {
            var postcode = await _context.PostalCodes
               .AsNoTracking()
               .Where(p => p.City == message.Postcode.City)
               .Where(p => p.State == message.Postcode.State)
               .Where(p => p.PostalCode == message.Postcode.PostalCode)
               .SingleOrDefaultAsync();

            if (postcode == null) return false;

            return true;
        }
    }
}