///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />
(function (ko, $, tasks, skills, userSkills, isVolunteeredForActivity) {

    function RequiredSkill(userSkills, skill)
    {
        var obj = ko.utils.extend({}, skill);
        obj.IsUserSkill = userSkills && userSkills.length && userSkills.filter(function (userSkill) { return obj.Id === userSkill.Id }).length > 0;
        return obj;
    }

    function ActivityViewModel(tasks, skills, userSkills) {
        this.skills = skills.map(RequiredSkill.bind(null, userSkills));
        tasks = tasks.map(function (task) { task.NotCompleteReason = ko.observable(); return task; });
        this.tasks = ko.observableArray(tasks).filterBeforeDate("EndDateTime").textFilter(["Name", "Description"]);
        this.enrolled = ko.observable(isVolunteeredForActivity);
        this.errorUnenrolling = ko.observable(false);
    }

    var activityViewModel;

    ActivityViewModel.prototype = {
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
            activityViewModel.errorUnenrolling(false);
            $("#enrollUnenrollSpinner").show();
            $.ajax({
                type: "DELETE",
                url: '/api/activity/' + activity + '/signup',
                contentType: "application/json",
            }).then(function (data) {
                activityViewModel.enrolled(false);
                $("#enrollUnenrollSpinner").hide();
            }).fail(function (fail) {
                activityViewModel.errorUnenrolling(true);
                $("#enrollUnenrollSpinner").hide();
                console.log(fail);
            });
        }
    };
    
    activityViewModel = new ActivityViewModel(tasks, skills, userSkills);

    ko.applyBindings(activityViewModel);
})(ko, $, modelStuff.tasks, modelStuff.skills, modelStuff.userSkills, modelStuff.isVolunteeredForActivity);
