(function (ko, $, modelData) {

    function TeamViewModel(itineraryId, assignedTeamMembers, potentialTeamMembers) {
        var self = this;

        self.itineraryId = itineraryId;
        self.validationErrors = ko.observableArray([]);
        self.teamMembers = ko.observableArray(assignedTeamMembers);
        self.hasTeamMembers = ko.computed(function() {
            return self.teamMembers().length > 0;
        });
        self.potentialTeamMembers = ko.observableArray(potentialTeamMembers);
        self.hasPotentialTeamMembers = ko.computed(function() {
            return self.potentialTeamMembers().filter(x => x.Value !== "").length > 0;
        });

        self.SelectedTeamMember = ko.observable().isRequired();

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

        self.submitForm = function() {
            self.isSubmitting(true);
            var dataToSend = {
                id: modelData.itineraryId,
                selectedTeamMember: self.SelectedTeamMember()
            };
            self.validationErrors([]);
            dataToSend.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
            $.ajax({
                type: "POST",
                url: "/admin/itinerary/addteammember",
                data: dataToSend,
                contentType: "application/x-www-form-urlencoded"
            }).done(function(result) {
                if (result.isSuccess) {
                    // TODO: respond
                } else {
                    self.validationErrors(result.errors);
                }
            }).fail(function(fail) {
                console.log(fail);
            }).always(function() {
                self.isSubmitting(false);
            });
        }
    }

    var teamViewModel = new TeamViewModel(modelData.itineraryId, modelData.assignedTeamMembers, modelData.potentialTeamMembers);
    ko.applyBindings(teamViewModel, document.getElementById("TeamView"));

})(ko, jQuery, modelData);