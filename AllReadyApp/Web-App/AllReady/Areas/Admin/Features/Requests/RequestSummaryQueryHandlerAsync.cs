using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AllReady.Areas.Admin.Models.RequestModels;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class RequestSummaryQueryHandlerAsync : IAsyncRequestHandler<RequestSummaryQuery, RequestSummaryModel>
    {
        private readonly AllReadyContext _context;

        public RequestSummaryQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<RequestSummaryModel> Handle(RequestSummaryQuery message)
        {
            return await _context.Requests.AsNoTracking()
                .Where(x => x.RequestId == message.RequestId)
                .Select(
                    x =>
                        new RequestSummaryModel
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
