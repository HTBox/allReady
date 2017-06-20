using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class CampaignManagerInviteDetailQueryHandler : IAsyncRequestHandler<CampaignManagerInviteDetailQuery, CampaignManagerInviteDetailsViewModel>
    {
        private AllReadyContext _context;

        public CampaignManagerInviteDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<CampaignManagerInviteDetailsViewModel> Handle(CampaignManagerInviteDetailQuery message)
        {
            var invite = await _context.CampaignManagerInvites.AsNoTracking()
                .Include(i => i.Campaign)
                .Include(i => i.SenderUser)
                .SingleOrDefaultAsync(i => i.Id == message.CampaignManagerInviteId);

            return new CampaignManagerInviteDetailsViewModel
            {
                Id = invite.Id,
                CampaignId = invite.CampaignId,
                CampaignName = invite.Campaign.Name,
                CustomMessage = invite.CustomMessage,
                AcceptedDateTimeUtc = invite.AcceptedDateTimeUtc,
                InviteeEmailAddress = invite.InviteeEmailAddress,
                IsAccepted = invite.IsAccepted,
                IsPending = invite.IsPending,
                IsRejected = invite.IsRejected,
                IsRevoked = invite.IsRevoked,
                RejectedDateTimeUtc = invite.RejectedDateTimeUtc,
                RevokedDateTimeUtc = invite.RevokedDateTimeUtc,
                SenderUserEmail = invite.SenderUser.Email,
                SentDateTimeUtc = invite.SentDateTimeUtc,
                OrganizationId = invite.Campaign.ManagingOrganizationId,
            };
        }
    }
}
