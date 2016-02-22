namespace AllReady.Security
{
    /// <summary>
    /// Custom claims types for HTBox AllReady
    /// </summary>
    public class ClaimTypes
    {
        /// <summary>
        /// The id of the organization
        /// </summary>
        public const string Organization = "ar:organizationid";

        /// <summary>
        /// The type of user
        /// </summary>
        public const string UserType = "ar:usertype";

        /// <summary>
        /// The display name of the user
        /// </summary>
        public const string DisplayName = "ar:displayname";

        /// <summary>
        /// The user's local time zone
        /// </summary>
        public const string TimeZoneId = "ar:timezoneid";

        /// <summary>
        /// A claim indicating that the user's profile is incomplete
        /// </summary>
        public const string ProfileIncomplete = "ar:profileincomplete";
    }
}
