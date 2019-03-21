using AllReady.Api.Data;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllReady.Api.Features.Commands
{
    public class CreateCampaignCommand : IRequest
    {
        public CreateCampaignCommand(Campaign campaign)
        {
            if (campaign is null)
                throw new ArgumentNullException(nameof(campaign));

            Campaign = campaign;
        }

        public Campaign Campaign { get; }
    }

    public class CreateCampaignCommandHandler : AsyncRequestHandler<CreateCampaignCommand>
    {
        private readonly AllReadyDbContext _dbContext;

        public CreateCampaignCommandHandler(AllReadyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
        {
            _dbContext.Add(request.Campaign);

            var updated = await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
