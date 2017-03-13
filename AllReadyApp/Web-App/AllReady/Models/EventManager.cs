namespace AllReady.Models
{
    /// <summary>
    /// Defines Event Managers via a many-to-many relationship between Users and Events
    /// </summary>
    public class EventManager
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
