namespace AllReady.Areas.Admin.ViewModels.Itinerary
{
    /// <summary>
    /// Defines a model used for optimize route post requests
    /// </summary>
    public class OptimizeRouteInputModel
    {
        /// <summary>
        /// The start address for route request optimization
        /// </summary>
        public string StartAddress { get; set; }

        /// <summary>
        /// The end address for route request optimization
        /// </summary>
        public string EndAddress { get; set; }

        /// <summary>
        /// Indicates that the end address should be the same as the start address for route optimization
        /// </summary>
        public bool EndSameAsStart { get; set; } = true;
    }
}
