using System.Collections.Generic;

namespace AllReady.ViewModels.Requests
{
    public class RequestViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // allow for unique identifiers and mapping information
        public string ProviderId { get; set; }      // for RedCross, "serial"
        public string ProviderData { get; set; }    // for Red Cross, "assigned_rc_region"

        public List<CommuniationPreference> CommunicationPrefernces { get; set; }
    }

    public enum CommuniationPreference
    {
        Sms = 0,
        Email = 1,
        Phone = 2
    }
}