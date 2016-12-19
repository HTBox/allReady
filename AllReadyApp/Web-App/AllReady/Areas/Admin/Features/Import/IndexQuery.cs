using AllReady.Areas.Admin.ViewModels.Import;
using MediatR;

namespace AllReady.Areas.Admin.Features.Import
{
    public class IndexQuery : IRequest<IndexViewModel>
    {
        public int? OrganizationId { get; set; }
    }
}
