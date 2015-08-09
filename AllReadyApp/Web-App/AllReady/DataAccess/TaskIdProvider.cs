using System.Threading;

namespace AllReady.DataAccess
{ /// <summary>
  /// Internal class for providing a theadsaf counter
  /// </summary>
    sealed class TaskIdProvider : ITaskIdProvider
    {
        private int current = 0;
        public int NextValue()
        {
            return Interlocked.Increment(ref this.current);
        }
        public void Reset()
        {
            this.current = 0;
        }
    }
}
