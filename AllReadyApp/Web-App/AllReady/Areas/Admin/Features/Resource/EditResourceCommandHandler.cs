using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Extensions;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Resource
{
    public class EditResourceCommandHandler : IAsyncRequestHandler<EditResourceCommand, int>
    {
        private AllReadyContext _context;

        public EditResourceCommandHandler(AllReadyContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(EditResourceCommand message)
        {
            var resource = await GetResource(message) ?? new AllReady.Models.Resource();

            resource.Name = message.Resource.Name;
            resource.Description = message.Resource.Description;
            resource.CampaignId = message.Resource.CampaignId;
            resource.ResourceUrl = message.Resource.ResourceUrl;

            _context.AddOrUpdate(resource);
            await _context.SaveChangesAsync();

            return resource.Id;
        }

        private Task<AllReady.Models.Resource> GetResource(EditResourceCommand message)
        {
            return _context.Resources.SingleOrDefaultAsync(r => r.Id == message.Resource.Id);
        }
    }
}
