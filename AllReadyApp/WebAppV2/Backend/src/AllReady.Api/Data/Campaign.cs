using NodaTime;
using System;

namespace AllReady.Api.Data
{
    public class Campaign
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string FullDescription { get; set; }

        public string ExternalUrl { get; set; }

        public string ExternalUrlText { get; set; }

        //public string Headline { get; set; }

        public string ImageUrl { get; set; }

        public LocalDate StartDateTime { get; set; }

        public LocalDate EndDateTime { get; set; }

        public DateTimeZone TimeZone { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public bool IsFeatured { get; set; }

        //public bool Published { get; set; }
    }
}
