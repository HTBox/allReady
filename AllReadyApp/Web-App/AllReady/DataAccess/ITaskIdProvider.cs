namespace AllReady.DataAccess
{
    public interface ITaskIdProvider
    {
        int NextValue();
        void Reset();
    }
}