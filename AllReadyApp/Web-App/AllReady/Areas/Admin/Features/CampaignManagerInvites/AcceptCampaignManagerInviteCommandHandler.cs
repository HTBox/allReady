using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class AcceptCampaignManagerInviteCommandHandler : AsyncRequestHandler<AcceptCampaignManagerInviteCommand>
    {
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public AcceptCampaignManagerInviteCommandHandler(AllReadyContext context, IMediator mediator, IUrlHelper urlHelper)
        {
            _context = context;
            _mediator = mediator;
            _urlHelper = urlHelper;
        }

        protected override async Task HandleCore(AcceptCampaignManagerInviteCommand message)
        {
            var invite = _context.CampaignManagerInvites
                .Include(c => c.Campaign)
                .Include(c => c.SenderUser)
                .Single(i => i.Id == message.CampaignManagerInviteId);
            invite.AcceptedDateTimeUtc = DateTimeUtcNow();
            await _context.SaveChangesAsync();
            await _mediator.PublishAsync(new CampaignManagerInviteAccepted
            {
                CampaignName = invite.Campaign.Name,
                InviteeEmail = invite.InviteeEmailAddress,
                CampaignUrl = _urlHelper.Link("CampaignDetails", new { id = invite.CampaignId }),
                SenderEmail = invite.SenderUser.Email
            });
        }
    }
}
