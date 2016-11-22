namespace AllReady.ViewModels.Requests
{
    public class RequestViewModel
    {
        //TODO: remove the RequestId from the ViewModel b/c we will never receive an incoming requset with our internal ID on it (at least I hope we won't)
        public string RequestId { get; set; }

        // incoming Red Cross "serial" field is mapped to this field, which will represnt the unique id of the request from an external provider
        public string ProviderId { get; set; }

        // incoming Red Cross "assigned_rc_region" field is mapped to this field. This field contains a red cross region and that region's zip code
        //an example of an rc_region value from RC is IDMT for "rc_idaho_montana"
        public string ProviderData { get; set; }

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
    }
}