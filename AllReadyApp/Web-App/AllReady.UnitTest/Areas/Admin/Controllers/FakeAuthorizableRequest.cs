using AllReady.Security;
using System;
using System.Threading.Tasks;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class FakeAuthorizableRequest : IAuthorizableRequest
    {
        private readonly bool _canView;
        private readonly bool _canEdit;
        private readonly bool _canDelete;
        private readonly bool _canManageChildren;

        public Guid RequestId { get; }
        public int ItineraryId { get; }
        public int EventId { get; }
        public int CampaignId { get; }
        public int OrganizationId { get; }

        public FakeAuthorizableRequest(bool canView, bool canEdit, bool canDelete, bool canManageChildren, int campaignId = 0)
        {
            _canView = canView;
            _canEdit = canEdit;
            _canDelete = canDelete;
            _canManageChildren = canManageChildren;

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
    }
}
