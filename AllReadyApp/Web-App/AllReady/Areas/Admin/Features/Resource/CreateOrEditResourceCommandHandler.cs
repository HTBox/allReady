using System.Threading.Tasks;
using AllReady.Extensions;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Resource
{
    public class CreateOrEditResourceCommandHandler : IAsyncRequestHandler<CreateOrEditResourceCommand, int>
    {
        private AllReadyContext _context; 

        public CreateOrEditResourceCommandHandler(AllReadyContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(CreateOrEditResourceCommand message)
        {
            var resource = await GetResource(message) ?? _context.Add(new AllReady.Models.Resource()).Entity;

            resource.Name = message.Resource.Name;
            resource.Description = message.Resource.Description;
            resource.CampaignId = message.Resource.CampaignId;
            resource.ResourceUrl = message.Resource.ResourceUrl;

            await _context.SaveChangesAsync();

            return resource.Id;
        }

        private Task<AllReady.Models.Resource> GetResource(CreateOrEditResourceCommand message)
        {
            return _context.Resources.SingleOrDefaultAsync(r => r.Id == message.Resource.Id);
        }
    }
}
