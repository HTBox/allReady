using System;

namespace AllReady.Security
{
    /// <summary>
    /// Contains the IDs that form full knowledge of components required to establish authorization on an <see cref="IAuthorizableRequest"/>
    /// </summary>
    /// <remarks>This is used for caching of the results of an <see cref="IAuthorizableRequest"/></remarks>
    public interface IAuthorizableRequestIdContainer
    {
        Guid RequestId { get; }
        int ItineraryId { get; }
        int EventId { get; }
        int CampaignId { get; }        
        int OrganizationId { get; }
    }
}