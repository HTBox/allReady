using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class RemoveTeamMemberCommandHandlerAsync : IAsyncRequestHandler<RemoveTeamMemberCommand, bool>
    {
        private readonly AllReadyContext _context;

        public RemoveTeamMemberCommandHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RemoveTeamMemberCommand message)
        {
            var taskSignup = await _context.TaskSignups
                .FirstOrDefaultAsync(x => x.Id == message.TaskSignupId).ConfigureAwait(false);

            if (taskSignup == null)
            {
                return false;
            }

            taskSignup.ItineraryId = null;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            
            return true;
        }
    }
}