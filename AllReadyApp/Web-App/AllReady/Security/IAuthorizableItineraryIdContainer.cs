namespace AllReady.Security
{
    /// <summary>
    /// Contains the IDs that form full knowledge of components required to establish authorization on an <see cref="IAuthorizableItinerary"/>
    /// </summary>
    /// <remarks>This is used for caching of the results of an <see cref="IAuthorizableItinerary"/></remarks>
    public interface IAuthorizableItineraryIdContainer
    {
        int ItineraryId { get; }
        int EventId { get; }
        int CampaignId { get; }        
        int OrganizationId { get; }
    }
}