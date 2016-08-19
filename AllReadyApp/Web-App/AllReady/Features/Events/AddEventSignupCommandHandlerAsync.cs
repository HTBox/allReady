using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Events
{
    public class AddEventSignupCommandHandlerAsync : AsyncRequestHandler<AddEventSignupCommandAsync>
    {
        private readonly AllReadyContext _context;

        public AddEventSignupCommandHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(AddEventSignupCommandAsync message)
        {
            _context.EventSignup.Add(message.EventSignup);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
