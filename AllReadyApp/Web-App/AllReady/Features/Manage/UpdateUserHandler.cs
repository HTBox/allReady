using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Manage
{
    public class UpdateUserHandler : AsyncRequestHandler<UpdateUser>
    {
        private readonly AllReadyContext dataContext;

        public UpdateUserHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        protected override async Task HandleCore(UpdateUser message) {
            var value = message.User;
            //First remove any skills that are no longer associated with this user
            var usksToRemove = this.dataContext.UserSkills.Where(usk => usk.UserId == value.Id && (value.AssociatedSkills == null ||
                !value.AssociatedSkills.Any(usk1 => usk1.SkillId == usk.SkillId)));
            this.dataContext.UserSkills.RemoveRange(usksToRemove);
            this.dataContext.Users.Update(value);
            await this.dataContext.SaveChangesAsync();
        }
    }
}
