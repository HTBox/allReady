using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class DeleteQueryHandler : IAsyncRequestHandler<DeleteQuery, DeleteViewModel>
    {
        private readonly AllReadyContext _context;

        public DeleteQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<DeleteViewModel> Handle(DeleteQuery message)
        {
            return await _context.Organizations.AsNoTracking()
                .Select(o => new DeleteViewModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    LogoUrl = o.LogoUrl,
                    WebUrl = o.WebUrl
                })
            .SingleAsync(x => x.Id == message.OrgId);
        }
    }
}
