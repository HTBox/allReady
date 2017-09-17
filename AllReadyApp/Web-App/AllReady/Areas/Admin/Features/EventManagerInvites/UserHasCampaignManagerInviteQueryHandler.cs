﻿using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class UserHasCampaignManagerInviteQueryHandler : IAsyncRequestHandler<UserHasEventManagerInviteQuery, bool>
    {
        private AllReadyContext _context;

        public UserHasCampaignManagerInviteQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UserHasEventManagerInviteQuery message)
        {
            return await _context.EventManagerInvites.AsNoTracking()
                .AnyAsync(i => i.InviteeEmailAddress == message.InviteeEmail && i.EventId == message.EventId);
        }
    }
}
