using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Resource;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Resource
{
    public class ResourceDetailQueryHandler : IAsyncRequestHandler<ResourceDetailQuery, ResourceDetailViewModel>
    {
        private readonly AllReadyContext _context;

        public ResourceDetailQueryHandler(AllReadyContext context)
        {
            _context = context; 
        }

        public async Task<ResourceDetailViewModel> Handle(ResourceDetailQuery message)
        {
            var resource = await _context.Resources.Include(r => r.Campaign).AsNoTracking().SingleAsync(r => r.Id == message.ResourceId);

            return new ResourceDetailViewModel(resource);
        }
    }
}
