namespace AllReady.Models
{
    public class TenantContact
    {
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public Contact Contact { get; set; }
        public int ContactId { get; set; }
        public int /*ContactType*/ ContactType { get; set; }
    }
}
