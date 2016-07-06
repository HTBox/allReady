﻿using AllReady.Models;
using MediatR;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationNameUniqueQueryHandlerAsync : IAsyncRequestHandler<OrganizationNameUniqueQuery, bool>
    {
        private AllReadyContext _context;

        public OrganizationNameUniqueQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(OrganizationNameUniqueQuery message)
        {
            var existingOrgCount = await _context.Organizations
                .CountAsync(o => o.Name == message.OrganizationName && o.Id != message.OrganizationId)
                .ConfigureAwait(false);

            return existingOrgCount <= 0;
        }
    }
}
