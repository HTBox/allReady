using MediatR;

namespace AllReady.Areas.Admin.Features.Resource
{
    public class DeleteResourceCommand : IAsyncRequest
    {
        public int ResourceId { get; set; } 
    }
}