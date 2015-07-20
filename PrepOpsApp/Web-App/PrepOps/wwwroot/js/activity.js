(function (ko, $, navigator) {

    var activityViewModel = {
        enrolled: ko.observable(false),
        enroll: function (activity) {
            // TODO: set up a spinner
            $.ajax({
                type: "POST",
                url: '/api/activity/' + activity + '/signup',
                contentType: "application/json",
            }).then(function (data) {
                activityViewModel.enrolled(true);
            }).fail(function (fail) {
                console.log(fail);
            });
        },
        unenroll: function (activity) {
            // TODO: set up a spinner
            $.ajax({
                type: "DELETE",
                url: '/api/activity/' + activity + '/signup',
                contentType: "application/json",
            }).then(function (data) {
                activityViewModel.enrolled(false);
            }).fail(function (fail) {
                console.log(fail);
            });
        }
    };
    ko.applyBindings(activityViewModel);
})(ko, $, window.navigator);