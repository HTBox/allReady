///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />
(function (ko, $, modelStuff) {

    function withIsUserSkill(userSkills, skill)
    {
        var obj = ko.utils.extend({}, skill);
        obj.IsUserSkill = ko.observable(userSkills && userSkills.length && userSkills.some(function (userSkill) {
            return obj.Id === userSkill.Id;
        }));
        return obj;
    }

    function EventViewModel(tasks, userTasks, eventSkills, userSkills, signupModel, isSignedIn, loginUrl) {
        var self = this;
        self.isSignedIn = isSignedIn;
        self.loginUrl = loginUrl;

        self.userSkills = userSkills;
        self.eventSkillsWithIsUser = ko.observableArray(eventSkills.map(withIsUserSkill.bind(null, userSkills)));
        self.unassociatedSkills = ko.computed(function() {
            return self.eventSkillsWithIsUser().filter(function(skill) {
                return !skill.IsUserSkill();
            });
        });

        self.tasks = ko.observableArray(tasks);
        self.filteredTasks = ko.computed(function() {
            return self.tasks
                .filterBeforeDate("EndDateTime")
                .textFilter(["Name", "Description"])
                .filterOnDateRange("StartDateTime", "EndDateTime");
        });

        self.userTasks = ko.observableArray(userTasks.map(function(task) {
            task.NotCompleteReason = ko.observable();
            return task;
        }));
        self.filteredUserTasks = ko.computed(function() {
            return self.userTasks
                .filterBeforeDate("EndDateTime")
                .textFilter(["Name", "Description"])
                .filterOnDateRange("StartDateTime", "EndDateTime");
        });

        self.tasks().forEach(function(task) {
            initializeTask(task);
        });

        self.userTasks().forEach(function(task) {
            initializeTask(task);
        });

        function initializeTask(task) {
            task.skillsWithIsUser = ko.observableArray(task.RequiredSkillObjects.map(withIsUserSkill.bind(null, userSkills)));
            task.unassociatedSkills = ko.computed(function() {
                return task.skillsWithIsUser().filter(function(skill) {
                    return !skill.IsUserSkill();
                });
            });
            task.hasAtLeastOneMatchingSkill = ko.computed(function() {
                return (task.skillsWithIsUser().length - task.unassociatedSkills().length) > 0;
            });
            task.hasAllMatchingSkills = ko.computed(function() {
                return (task.unassociatedSkills().length) === 0;
            });
            task.isExpanded = ko.observable(false);
            if (task.AssignedVolunteers[0]) {
                task.currentStatus = task.AssignedVolunteers[0].Status;
            }
        }

        self.enrolled = ko.observable(modelStuff.isVolunteeredForEvent);
        self.errorUnenrolling = ko.observable(false);

        self.signupForTask = function (task) {
            hideAlert();
            var vm = new SignupViewModel(signupModel, task.unassociatedSkills, task.Name, task);
            vm.modal = HTBox.showModal({ viewModel: vm, modalId: "VolunteerModal" })
                .onClose(taskSignupSuccess);
        };

        function taskSignupSuccess(signUpViewModel) {
            initializeTask(signUpViewModel.UpdatedTask);
            self.userTasks.push(signUpViewModel.UpdatedTask);
            self.tasks.remove(signUpViewModel.Task);
            if (signUpViewModel.AddSkillIds().length) {
                updateTaskSkillsWithNewUserSkills(self.tasks(), signUpViewModel.AddSkillIds());
                updateTaskSkillsWithNewUserSkills(self.userTasks(), signUpViewModel.AddSkillIds());
            };
            showalert("<strong>Thanks for volunteering! Your request has been processed and one of the event coordinators will be in contact with you soon.</strong>", "alert-success", 30);
        }

        function updateTaskSkillsWithNewUserSkills(tasks, newSkillIds) {
            tasks.forEach(function(task) {
                task.skillsWithIsUser().forEach(function(skill) {
                    if (newSkillIds.indexOf(skill.Id) >= 0) {
                        skill.IsUserSkill(true);
                    }
                });
            });
        }

        self.changeTaskStatus = function (newStatus, statusDescription, task) {
            hideAlert();
            self.errorUnenrolling(false);
            $("#enrollUnenrollSpinner").show();
            var viewModel = new TaskStatusChangeViewModel(task.Id, task.AssignedVolunteers[0].UserId, newStatus, statusDescription);
            viewModel.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
            $.ajax({
                type: "POST",
                url: '/api/task/changestatus',
                data: viewModel,
                contentType: "application/x-www-form-urlencoded"
            }).then(function(result) {
                self.userTasks.remove(task);
                initializeTask(result.Task);
                self.userTasks.push(result.Task);
                $("#enrollUnenrollSpinner").hide();
                showalert("<strong>Your request to change the status of task: '" + task.Name + "' has been successfully processed.</strong>", "alert-success", 10);
            }).fail(function(fail) {
                self.errorUnenrolling(true);
                console.log(fail);
            });
        }

        self.confirmUnenrollFromTask = function (task) {
            hideAlert();
            var taskSave = task;
            var title = "Confirm";
            var body = "Please confirm that you want to unenroll from the task:<br><strong>" + task.Name + "</strong>";
            var vm = new ConfirmViewModel(title, body);
            vm.modal = HTBox.showModal({ viewModel: vm, modalId: "ConfirmModal" })
                .onClose(function(vm) {
                    self.unenrollFromTask(taskSave);
                });
        };

        self.unenrollFromTask = function (task) {
            hideAlert();
            self.errorUnenrolling(false);
            $("#enrollUnenrollSpinner").show();
            $.ajax({
                type: "DELETE",
                url: '/api/task/' + task.Id + '/signup',
                contentType: "application/json"
            }).then(function(result) {
                initializeTask(result.Task);
                self.tasks.push(result.Task);
                self.userTasks.remove(task);
                $("#enrollUnenrollSpinner").hide();
                showalert("<strong>Thanks for your interest. Your request has been processed and you are no longer signed up for this task. We hope to see you soon!.</strong>", "alert-success", 30);
            }).fail(function(fail) {
                self.errorUnenrolling(true);
                console.log(fail);
            });
        }

        self.confirmCannotComplete = function (task) {
            hideAlert();
            var taskSave = task;
            var vm = new CannotCompleteViewModel(task);
            vm.modal = HTBox.showModal({ viewModel: vm, modalId: "CannotCompleteModal" })
                .onClose(function (viewModel) {
                    self.changeTaskStatus("CanNotComplete", viewModel.NotCompleteReason, taskSave);
                });
        };

        self.confirmGoToLogin = function () {
            hideAlert();
            var title = "Confirm";
            var body = "In order to volunteer for this opportunity you must login to an existing account or register to create a new account. Please click confirm to go to the login page.";
            var vm = new ConfirmViewModel(title, body);
            vm.modal = HTBox.showModal({ viewModel: vm, modalId: "ConfirmModal" })
                .onClose(function(vm) {
                    window.location = self.loginUrl;
                });
        };

        self.sortStartDateTimeAsc = function(l, r) {
            return l.StartDateTime > r.StartDateTime ? 1 : -1;
        };

        self.alertVm = new AlertViewModel();

        function showalert(message, alertType, timeoutInSecs) {
            self.alertVm.message(message);
            self.alertVm.type(alertType);
            self.alertVm.visible(true);
            clearTimeout(self.alertVm.timer);
            if (timeoutInSecs) {
                self.alertVm.timer = setTimeout(function() {
                    self.alertVm.visible(false);
                }, timeoutInSecs * 1000);
            };
        };

        function hideAlert() {
            self.alertVm.visible(false);
        }
    };

    SignupViewModel = function (signupModelSeed, unassociatedSkills, title, task) {
        var self = this;
        ko.mapping.fromJS(signupModelSeed, {}, self);
        self.unassociatedSkills = unassociatedSkills;
        self.validationErrors = ko.observableArray([]);

        if (task) {
            self.VolunteerTaskId = task.Id;
            self.Task = task;
        }

        self.Heading = "Volunteer for " + title;

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

        self.isSubmitting = ko.observable(false);

        self.submitForm = function () {
            // TODO: set up a spinner
            self.isSubmitting(true);
            var dataToSend = ko.mapping.toJS(self);
            self.validationErrors([]);
            dataToSend.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
            $.ajax({
                type: "POST",
                url: "/api/task/signup",
                data: dataToSend,
                contentType: "application/x-www-form-urlencoded"
            }).done(function (result) {
                self.isSubmitting(false);
                switch (result.isSuccess) {
                    case true:
                        self.UpdatedTask = result.task;
                        self.modal.close(self);
                        break;
                    case false:
                        self.validationErrors(result.errors);
                        break;
                    default:
                }
            }).fail(function(fail) {
                self.isSubmitting(false);
                self.validationErrors([fail.statusText]);
                console.log(fail);
            });
        }

        self.cancel = function () {
            self.modal.cancel();
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

   CannotCompleteViewModel = function (task) {
        var self = this;
        self.Name = task.Name;
        self.NotCompleteReason = ko.observable("");

        self.submit = function () {
            self.modal.close(self);
        }

        self.cancel = function() {
            self.modal.cancel();
        }
    }

    ConfirmViewModel = function (title, body) {
        var self = this;
        self.title = title;
        self.body = body;

        self.submit = function () {
            self.modal.close(self);
        }

        self.cancel = function () {
            self.modal.cancel();
        }
    }

    TaskStatusChangeViewModel = function(volunteerTaskId, userId, status, statusDescription) {
        var self = this;
        self.VolunteerTaskId = volunteerTaskId;
        self.UserId = userId;
        self.Status = status;
        self.StatusDescription = statusDescription;
    };

    var eventViewModel = new EventViewModel(modelStuff.tasks, modelStuff.userTasks, modelStuff.eventSkills, modelStuff.userSkills, modelStuff.signupModelSeed, modelStuff.isSignedIn, modelStuff.loginUrl);
    ko.applyBindings(eventViewModel, document.getElementById("MainView"));
})(ko, $, modelStuff);
