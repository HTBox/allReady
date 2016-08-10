using System.Linq;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        private readonly AllReadyContext _dbContext;

        public AllReadyDataAccessEF7( AllReadyContext dbContext )
        {
            _dbContext = dbContext;
        }
    }
}
