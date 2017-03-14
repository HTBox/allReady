namespace AllReady.Security
{
    public interface IAuthorizableEvent
    {
        int CampaignId { get; }
        int EventId { get; }
        int OrganizationId { get; }
    }
}