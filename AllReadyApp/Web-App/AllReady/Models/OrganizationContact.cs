namespace AllReady.Models
{
    public class OrganizationContact
    {
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public Contact Contact { get; set; }
        public int ContactId { get; set; }
        public int /*ContactType*/ ContactType { get; set; }
    }
}
