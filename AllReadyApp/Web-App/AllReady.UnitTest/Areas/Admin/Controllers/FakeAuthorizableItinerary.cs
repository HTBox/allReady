using AllReady.Security;
using System.Threading.Tasks;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class FakeAuthorizableItinerary : IAuthorizableItinerary
    {
        private readonly bool _canView;
        private readonly bool _canEdit;
        private readonly bool _canDelete;
        private readonly bool _canManageChildren;
        private readonly bool _canManageRequests;
        private readonly bool _canManageTeamMembers;

        public int ItineraryId { get; }
        public int EventId { get; }
        public int CampaignId { get; }
        public int OrganizationId { get; }

        public FakeAuthorizableItinerary(bool canView, bool canEdit, bool canDelete, bool canManageChildren, bool canManageRequests, bool canManageTeamMembers, int campaignId = 0)
        {
            _canView = canView;
            _canEdit = canEdit;
            _canDelete = canDelete;
            _canManageChildren = canManageChildren;
            _canManageRequests = canManageRequests;
            _canManageTeamMembers = canManageTeamMembers;

            CampaignId = campaignId;
        }

        public Task<bool> UserCanView()
        {
            return Task.FromResult(_canView);
        }

        public Task<bool> UserCanEdit()
        {
            return Task.FromResult(_canEdit);
        }

        public Task<bool> UserCanDelete()
        {
            return Task.FromResult(_canDelete);
        }

        public Task<bool> UserCanManageChildObjects()
        {
            return Task.FromResult(_canManageChildren);
        }

        public Task<bool> UserCanManageRequests()
        {
            return Task.FromResult(_canManageRequests);
        }

        public Task<bool> UserCanManageTeamMembers()
        {
            return Task.FromResult(_canManageTeamMembers);
        }
    }
}
