namespace AllReady.Security.AuthoizationHandlers
{
    public class ItineraryAuthorizationStub
    {
        public ItineraryAuthorizationStub(int id)
        {
            ItineraryId = id;
        }

        public int ItineraryId { get; private set; }
    }
}
