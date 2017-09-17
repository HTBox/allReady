namespace AllReady.Security
{
    public interface IAuthorizableCampaignIdContainer
    {
        int CampaignId { get; }
        int OrganizationId { get; }
    }
}
