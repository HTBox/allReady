using System.Linq;
using AllReady.Areas.Admin.ViewModels.Site;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Site
{
    public class IndexQueryHandler : IRequestHandler<IndexQuery, IndexViewModel>
    {
        private readonly AllReadyContext _context;

        public IndexQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public IndexViewModel Handle(IndexQuery message)
        {
            return new IndexViewModel
            {
                Users = _context.Users
                    .OrderBy(u => u.UserName)
                    .ToList()
            };
        }
    }
}