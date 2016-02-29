///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />
(function (ko, $, tasks, skills, userSkills, isVolunteeredForActivity, signupModelSeed) {

    function RequiredSkill(userSkills, skill)
    {
        var obj = ko.utils.extend({}, skill);
        obj.IsUserSkill = ko.observable(userSkills && userSkills.length && userSkills.filter(function (userSkill) { return obj.Id === userSkill.Id }).length > 0);
        return obj;
    }

    function ActivityViewModel(tasks, skills, userSkills, signupModel) {
        var self = this;
        self.skills = ko.observableArray(skills.map(RequiredSkill.bind(null, userSkills)));
        self.unassociatedSkills = ko.computed(function () { return self.skills().filter(function (skill) { return !skill.IsUserSkill(); }) }); 
        tasks = tasks.map(function (task) { task.NotCompleteReason = ko.observable(); return task; });
        self.tasks = ko.observableArray(tasks).filterBeforeDate("EndDateTime").textFilter(["Name", "Description"]);
        self.enrolled = ko.observable(isVolunteeredForActivity);
        self.errorUnenrolling = ko.observable(false);

        self.signupForActivity = function () {
            var vm = new SignupViewModel(signupModel, self.unassociatedSkills);
            vm.modal = HTBox.showModal({
                viewModel: vm,
                modalId: "VolunteerModal"
            }).onClose(activitySignupSuccess);
        };

        function activitySignupSuccess(signUpViewModel) {
            self.skills().forEach(function(skill) {
                if (signUpViewModel.AddSkillIds().indexOf(skill.Id) >= 0){ skill.IsUserSkill(true) }
            });
            self.enrolled(true);
            showalert("<strong>Thanks for volunteering! Your request has been processed and one of the activity coordinators will be in contact with you soon.</strong>", "alert-success", 30);
        }

        self.unenroll = function (activity) {
            // TODO: set up a spinner
            self.errorUnenrolling(false);
            $("#enrollUnenrollSpinner").show();
            $.ajax({
                type: "DELETE",
                url: '/api/activity/' + activity + '/signup',
                contentType: "application/json"
            }).then(function (data) {
                self.enrolled(false);
                $("#enrollUnenrollSpinner").hide();
                showalert("<strong>Thanks for your interest. Your request has been processed and you are no longer signed up for this activity. We hope to see you soon!</strong>", "alert-success", 30);

            }).fail(function (fail) {
                self.errorUnenrolling(true);
                console.log(fail);
            });
        }
    }

    self.alertVm = new AlertViewModel();
    function showalert(message, alertType, timeoutInSecs) {
        self.alertVm.message(message);
        self.alertVm.type(alertType);
        self.alertVm.visible(true);
        clearTimeout(self.alertVm.timer);
        if (timeoutInSecs) {
            self.alertVm.timer = setTimeout(function () {
                self.alertVm.visible(false);
            }, timeoutInSecs * 1000);
        };
    };

    function SignupViewModel (signupModelSeed, unassociatedSkills) {
        var self = this;
        ko.mapping.fromJS(signupModelSeed, {}, self);
        self.unassociatedSkills = unassociatedSkills;
        self.validationErrors = ko.observableArray([]);

        self.PreferredEmail
            .isRequired()
            .validateEmail()
            .notifyChangeFromInitialValue();

        self.PreferredPhoneNumber
           .isRequired()
           .validatePhoneNumber()
           .notifyChangeFromInitialValue();

        self.isValid = ko.computed(function () {
            var allValidatablesAreValid = true;
            for (var property in self) {
                if (self.hasOwnProperty(property) && typeof self[property].isValid === "function") {
                    allValidatablesAreValid = self[property].isValid();
                }
                if (!allValidatablesAreValid) break;
            }
            return allValidatablesAreValid;
        });

        self.submitForm = function() {
            // TODO: set up a spinner
            var dataToSend = ko.mapping.toJS(self);
            self.validationErrors([]);
            dataToSend.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
            $.ajax({
                type: "POST",
                url: '/api/activity/signup',
                data: dataToSend,
                contentType: "application/x-www-form-urlencoded"
            }).done(function (data, status) {
                if (!data) {
                    self.modal.close(self);
                } else {
                    self.validationErrors(data.errors);
                }
            }).fail(function(fail) { console.log(fail); });
        }
    };

    function AlertViewModel() {
        var self = this;
        self.message = ko.observable('');
        self.type = ko.observable('');
        self.visible = ko.observable(false);
        self.close = function (alertVm) {
            alertVm.visible(false);
            clearTimeout(alertVm.timer);
        }
    }


    var activityViewModel = new ActivityViewModel(tasks, skills, userSkills, signupModelSeed);
    ko.applyBindings(activityViewModel, document.getElementById("MainView"));
})(ko, $, modelStuff.tasks, modelStuff.skills, modelStuff.userSkills, modelStuff.isVolunteeredForActivity, modelStuff.signupModelSeed);
