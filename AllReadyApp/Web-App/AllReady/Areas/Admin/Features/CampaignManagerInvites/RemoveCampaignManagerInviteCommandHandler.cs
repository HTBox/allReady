using System;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class RemoveCampaignManagerInviteCommandHandler : AsyncRequestHandler<RemoveCampaignManagerInviteCommand>
    {
        private readonly AllReadyContext _context;

        public RemoveCampaignManagerInviteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(RemoveCampaignManagerInviteCommand message)
        {
            var invite = await _context.CampaignManagerInvites.FirstOrDefaultAsync(x => x.Id == message.InviteId);
            if (invite == null)
                throw new InvalidOperationException($"Failed to find invite {message.InviteId}.");

            _context.CampaignManagerInvites.Remove(invite);
            await _context.SaveChangesAsync();
        }
    }
}
