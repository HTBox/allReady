namespace AllReady.Models
{
    public class CampaignContact
    {
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
        public Contact Contact { get; set; }
        public int ContactId { get; set; }
        public int /*ContactType*/ ContactType { get; set; }
    }

}
