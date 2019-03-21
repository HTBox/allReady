using NodaTime;

namespace AllReady.Api.Models.Output.Events
{
    public abstract class EventOutputModelBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public LocalDate StartDateTime { get; set; }

        public LocalDate EndDateTime { get; set; }

        public DateTimeZone TimeZone { get; set; }
    }
}
