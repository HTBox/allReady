namespace AllReady.Security
{
    /// <summary>
    /// Custom claims types for HTBox AllReady
    /// </summary>
    public class ClaimTypes
    {
        /// <summary>
        /// The id of the tenant
        /// </summary>
        public const string Tenant = "ar:tenantid";

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
    }
}
