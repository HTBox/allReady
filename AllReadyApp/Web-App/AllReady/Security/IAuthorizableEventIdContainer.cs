namespace AllReady.Security
{
    public interface IAuthorizableEventIdContainer
    {
        int CampaignId { get; }
        int EventId { get; }
        int OrganizationId { get; }
    }
}