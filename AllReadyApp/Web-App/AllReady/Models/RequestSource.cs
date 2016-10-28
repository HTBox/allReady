namespace AllReady.Models
{
    /// <summary>
    /// Describes the source of a request
    /// </summary>
    public enum RequestSource
    {
        /// <summary>
        /// Source unknown (default)
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Pushed into allReady via API
        /// </summary>
        Api = 1,

        /// <summary>
        /// Pulled into allReady via CSV
        /// </summary>
        Csv = 2,

        /// <summary>
        /// Manually created via admin UI
        /// </summary>
        Manual = 3
    }
}
