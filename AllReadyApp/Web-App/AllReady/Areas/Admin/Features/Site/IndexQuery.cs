using AllReady.Areas.Admin.ViewModels.Site;
using MediatR;

namespace AllReady.Areas.Admin.Features.Site
{
    public class IndexQuery : IRequest<IndexViewModel>
    {
    }
}
