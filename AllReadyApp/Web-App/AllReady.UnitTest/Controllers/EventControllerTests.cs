using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class EventControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private readonly Task taskFromResultZero = Task.FromResult(0);

        [Fact(Skip = "NotImplemented")]
        public void GetMyEventsSendsGetMyEventsQueryWithTheCorrectUserId()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyEventsReturnsTheCorrectViewAndViewModel()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyEventsHasRouteAttributeWithTheCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void GetMyEventsHasAuthorizeAttribute()
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
        public void ShowEventSendsShowEventQueryWithCorrectData()
        {

        }

        [Fact(Skip = "NotImplemented")]
        public void ShowEventReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowEventReturnsEventWithTasksViewWithCorrrectViewModelWhenViewModelIsNotNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowEventHasRouteAttributeWithCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ShowEventHasAllowAnonymousAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SignupReturnsBadRequestResultWhenViewModelIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SignupSendsAsyncEventSignupCommandWithCorrrectDataWhenViewModelIsNotNull()
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
