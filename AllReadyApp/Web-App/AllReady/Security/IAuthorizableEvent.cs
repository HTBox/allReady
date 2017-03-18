using System.Threading.Tasks;

namespace AllReady.Security
{
    public interface IAuthorizableEvent : IAuthorizable
    {
        int CampaignId { get; }
        int EventId { get; }
        int OrganizationId { get; }
        Task<EventAccessType> UserAccessType();
    }
}