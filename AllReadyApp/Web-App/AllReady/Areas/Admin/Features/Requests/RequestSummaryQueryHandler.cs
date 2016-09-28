using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class RequestSummaryQueryHandler : IAsyncRequestHandler<RequestSummaryQuery, RequestSummaryViewModel>
    {
        private readonly AllReadyContext _context;

        public RequestSummaryQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<RequestSummaryViewModel> Handle(RequestSummaryQuery message)
        {
            return await _context.Requests.AsNoTracking()
                .Where(x => x.RequestId == message.RequestId)
                .Select(
                    x =>
                        new RequestSummaryViewModel
                        {
                            Id = x.RequestId,
                            Name = x.Name,
                            Address = x.Address,
                            City = x.City,
                            State = x.State,
                            Status = x.Status
                        })
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }
    }
}
