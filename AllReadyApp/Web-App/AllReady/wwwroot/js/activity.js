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

        self.signupForActivity = function() {
             HTBox.showModal({
                 viewModel: new SignupViewModel(signupModel, self.unassociatedSkills),
                 template: "VolunteerModal",
                 context: self
             }).then(activitySignupSuccess);
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
                showalert("<strong>Thanks for your interest. Your request has been processed and you are no longer signed up for this activity We hope to see you soon!.</strong>", "alert-success", 30);
            }).fail(function (fail) {
                self.errorUnenrolling(true);
                console.log(fail);
            });
        }
    }

    function showalert(message, alerttype, timeoutInSecs) {
        $('#alert_placeholder').append('<div id="alertdiv" class="alert ' +  alerttype + ' fade in"><a class="close" data-dismiss="alert">×</a><span>'+message+'</span></div>')
        if (timeoutInSecs) {
            setTimeout(function() {
                $(".alert").alert('close');
            }, timeoutInSecs * 1000);
        };
    };

    function SignupViewModel (signupModelSeed, unassociatedSkills) {
        var self = this;
        ko.mapping.fromJS(signupModelSeed, {}, self);
        self.unassociatedSkills = unassociatedSkills;
        self.validationErrors = ko.observableArray([]);

        self.PreferredPhoneNumber.extend({
            validate: {
                required: "'Phone Number' is required",
                phoneNumber: ""
            }
        });

        self.PreferredEmail.extend({
            validate: {
                required: "'Email' is required",
                email: ""
            }
        });

        self.isValid = ko.computed(function () {
            var allValidatablesAreValid = true;
            for (var property in self) {
                if (self.hasOwnProperty(property) && self[property]["hasError"]) {
                    allValidatablesAreValid = !self[property].hasError();
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

    var activityViewModel = new ActivityViewModel(tasks, skills, userSkills, signupModelSeed);
    ko.applyBindings(activityViewModel);
})(ko, $, modelStuff.tasks, modelStuff.skills, modelStuff.userSkills, modelStuff.isVolunteeredForActivity, modelStuff.signupModelSeed);
