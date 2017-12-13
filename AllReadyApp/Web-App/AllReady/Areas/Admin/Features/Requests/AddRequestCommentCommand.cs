using System;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class AddRequestCommentCommand : IAsyncRequest
    {
        public AddRequestCommentCommand(string comment, string userId, Guid requestId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (requestId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(requestId));
            }

            RequestComment = comment ?? throw new ArgumentNullException(nameof(comment));
            UserId = userId;
            RequestId = requestId;
         
        }

        public string RequestComment { get; }
        public string UserId { get; }
        public Guid RequestId { get; }
    }

    public class AddRequestCommentCommandHandler : AsyncRequestHandler<AddRequestCommentCommand>
    {
        private readonly AllReadyContext _context;

        public AddRequestCommentCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(AddRequestCommentCommand message)
        {
            var comment = new RequestComment
            {
                Comment = message.RequestComment,
                CreatedUtcDate = DateTime.UtcNow,
                RequestId = message.RequestId,
                UserId = message.UserId
            };

            _context.RequestComments.Add(comment);

            await _context.SaveChangesAsync();
        }
    }
}
