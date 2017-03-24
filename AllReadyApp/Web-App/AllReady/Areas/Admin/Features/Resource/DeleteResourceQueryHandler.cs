using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Resource;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Resource
{
    public class DeleteResourceQueryHandler : IAsyncRequestHandler<DeleteResourceQuery, ResourceDeleteViewModel>
    {
        private readonly AllReadyContext _context;

        public DeleteResourceQueryHandler(AllReadyContext context)
        {
            _context = context; 
        }

        public async Task<ResourceDeleteViewModel> Handle(DeleteResourceQuery message)
        {
            var resource =  await _context.Resources.AsNoTracking().SingleAsync(r => r.Id == message.ResourceId);

            return new ResourceDeleteViewModel(resource);
        }
    }
}
