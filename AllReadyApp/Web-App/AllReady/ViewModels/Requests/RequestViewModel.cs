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

        // allow for unique identifiers and mapping information
        public string ProviderId { get; set; }      // for RedCross, "serial"
        public string ProviderData { get; set; }    // for Red Cross, "assigned_rc_region"
    }
}