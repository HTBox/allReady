using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class ActivityControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private readonly Task taskFromResultZero = Task.FromResult(0);

        [Fact(Skip = "NotImplemented")]
        public void GetMyActivitiesSendsGetMyActivitiesQueryWithTheCorrectUserId()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyActivitiesReturnsTheCorrectViewAndViewModel()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyActivitiesHasRouteAttributeWithTheCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyActivitiesHasAuthorizeAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyTasksSendsGetMyTasksQueryWithTheCorrectData()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyTasksReturnsCorrectJsonView()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyTasksHasRouteAttributeWithCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyTasksHasAuthorizeAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task UpdateMyTasksSendsUpdateMyTasksCommandAsyncWithCorrectData()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task UpdateMyTasksReturnsJsonResultWithTheCorrectData()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void UpdateMyTasksHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void UpdateMyTasksHasAuthorizeAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void UpdateMyTasksHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void UpdateMyTasksHasRouteAttributeWithCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void IndexReturnsTheCorrectView()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void IndexHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowActivitySendsShowActivityQueryWithCorrectData()
        {
            
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowActivityReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowActivityReturnsActivityViewWithCorrrectViewModelWhenViewModelIsNotNullAndActivityTypeIsActivityManaged()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowActivityReturnsActivityWithTasksViewWithCorrrectViewModelWhenViewModelIsNotNullAndActivityTypeIsNotActivityManaged()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowActivityHasRouteAttributeWithCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowActivityHasAllowAnonymousAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SignupReturnsBadRequestResultWhenViewModelIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SignupSendsAsyncActivitySignupCommandWithCorrrectDataWhenViewModelIsNotNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SignupRedirectsToCorrectActionWithCorrectRouteValuesWhenViewModelIsNotNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void SignupHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void SignupHasAuthorizeAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void SignupHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void SignupHasRouteAttributeWithCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeStatusReturnsBadRequestResultWhenUserIdIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeStatusSendsTaskStatusChangeCommandAsyncWithCorrectData()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeStatusRedirectsToCorrectActionWithCorrectRouteValues()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangeStatusHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangeStatusHasRouteAttributeWithCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangeStatusHasAuthorizeAttribute()
        {
        }
    }
}
