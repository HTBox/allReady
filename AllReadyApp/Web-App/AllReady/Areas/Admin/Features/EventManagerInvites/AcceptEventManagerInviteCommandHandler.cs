using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class AcceptEventManagerInviteCommandHandler : AsyncRequestHandler<AcceptEventManagerInviteCommand>
    {
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public AcceptEventManagerInviteCommandHandler(AllReadyContext context, IMediator mediator, IUrlHelper urlHelper)
        {
            _context = context;
            _mediator = mediator;
            _urlHelper = urlHelper;
        }

        protected override async Task HandleCore(AcceptEventManagerInviteCommand message)
        {
            var invite = _context.EventManagerInvites
                .Include(e => e.Event).ThenInclude(e => e.Campaign)
                .Include(e => e.SenderUser)
                .Single(i => i.Id == message.EventManagerInviteId);

            invite.AcceptedDateTimeUtc = DateTimeUtcNow();
            await _context.SaveChangesAsync();

            await _mediator.PublishAsync(new EventManagerInviteAccepted
            {
                EventName = invite.Event.Name,
                CampaignName = invite.Event.Campaign.Name,
                InviteeEmail = invite.InviteeEmailAddress,
                EventUrl = _urlHelper.Link("EventDetails", new { id = invite.EventId }),
                SenderEmail = invite.SenderUser.Email
            });
        }
    }
}
