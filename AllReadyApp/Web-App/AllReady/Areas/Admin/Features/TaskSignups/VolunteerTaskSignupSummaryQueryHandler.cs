using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.TaskSignups
{
    public class VolunteerTaskSignupSummaryQueryHandler : IAsyncRequestHandler<VolunteerTaskSignupSummaryQuery, VolunteerTaskSignupSummaryViewModel>
    {
        private readonly AllReadyContext _context;

        public VolunteerTaskSignupSummaryQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<VolunteerTaskSignupSummaryViewModel> Handle(VolunteerTaskSignupSummaryQuery message)
        {
            return await _context.VolunteerTaskSignups.AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.Id == message.VolunteerTaskSignupId)
                .Select( x =>
                new VolunteerTaskSignupSummaryViewModel
                {
                    VolunteerTaskSignupId = x.Id,
                    VolunteerName = x.User.Name,
                    VolunteerEmail = x.User.Email
                })
                .FirstOrDefaultAsync();
        }
    }
}