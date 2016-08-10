namespace AllReady.Features.Requests
{
    public class AddRequestError
    {
        public string ProviderId { get; set; }
        public string Reason { get; set; }
        public bool IsInternal { get; set; }
    }
}
