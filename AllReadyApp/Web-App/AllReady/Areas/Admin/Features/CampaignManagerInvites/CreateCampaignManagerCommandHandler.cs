using AllReady.Models;
using MediatR;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class CreateCampaignManagerCommandHandler : AsyncRequestHandler<CreateCampaignManagerCommand>
    {
        private readonly AllReadyContext _context;

        public CreateCampaignManagerCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(CreateCampaignManagerCommand message)
        {
            var userExist = _context.CampaignManagers.Any(e => e.UserId == message.UserId && e.CampaignId == message.CampaignId);

            if (userExist) return;

            _context.CampaignManagers.Add(new CampaignManager
            {
                CampaignId = message.CampaignId,
                UserId = message.UserId
            });

            await _context.SaveChangesAsync();
        }
    }
}
