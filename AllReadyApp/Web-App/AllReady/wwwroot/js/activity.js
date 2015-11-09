///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, tasks, skills, userSkills) {

    function RequiredSkill(userSkills, skill)
    {
        var obj = ko.utils.extend({}, skill);
        obj.IsUserSkill = userSkills && userSkills.length && userSkills.filter(function (userSkill) { return obj.Id === userSkill.Id }).length > 0;
        return obj;
    }

    function ActivityViewModel(tasks, skills, userSkills) {
        this.skills = skills.map(RequiredSkill.bind(null, userSkills));
        this.tasks = ko.observableArray(tasks).filterBeforeDate("EndDateTime").textFilter(["Name", "Description"]);
        this.enrolled = ko.observable(false);
    }

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
    
    ko.applyBindings(new ActivityViewModel(tasks, skills, userSkills));
})(ko, $, modelStuff.tasks, modelStuff.skills, modelStuff.userSkills);