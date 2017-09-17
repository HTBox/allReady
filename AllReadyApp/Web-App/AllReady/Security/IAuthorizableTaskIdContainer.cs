namespace AllReady.Security
{
    /// <summary>
    /// Contains the IDs that form full knowledge of components required to establish authorization on an <see cref="IAuthorizableTask"/>
    /// </summary>
    /// <remarks>This is used for caching of the results of an <see cref="IAuthorizableTask"/></remarks>
    public interface IAuthorizableTaskIdContainer
    {
        int TaskId { get; }
        int EventId { get; }
        int CampaignId { get; }
        int OrganizationId { get; }
    }
}