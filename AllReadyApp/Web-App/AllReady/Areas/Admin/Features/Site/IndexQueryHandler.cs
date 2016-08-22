using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Site;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Site
{
    public class IndexQueryHandler : IAsyncRequestHandler<IndexQuery, IndexViewModel>
    {
        private readonly AllReadyContext _context;

        public IndexQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IndexViewModel> Handle(IndexQuery message)
        {
            return new IndexViewModel
            {
                Users = await _context.Users
                    .OrderBy(u => u.UserName)
                    .ToListAsync()
                    .ConfigureAwait(false)
            };
        }
    }
}