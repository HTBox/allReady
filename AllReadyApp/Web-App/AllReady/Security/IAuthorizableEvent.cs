namespace AllReady.Security
{
    public interface IAuthorizableEvent : IAuthorizable
    {
        int CampaignId { get; }
        int EventId { get; }
        int OrganizationId { get; }
    }
}