namespace AllReady.Security
{
    public interface IAuthorizableTask : IAuthorizable
    {
        /// <summary>
        /// The ID of the task
        /// </summary>
        int TaskId { get; }

        /// <summary>
        /// The ID of the event that the task belongs to
        /// </summary>
        int EventId { get; }

        /// <summary>
        /// The ID of the campaign that the task belongs to
        /// </summary>
        int CampaignId { get; }

        /// <summary>
        /// The ID of the organization that the task belongs to
        /// </summary>
        int OrganizationId { get; }
    }
}
