(function (ko, $) {
    function TaskModel(status, statusDescription, id, name, tid) {
        this.status = ko.observable(status);
        this.statusDescription = ko.observable(statusDescription);
        this.id = ko.observable(id);
        this.name = ko.observable(name);
        this.taskId = ko.observable(tid);
    }
    var myActivitiesViewModel = {
        tasksLoaded: ko.observable(false),
        statusList: ko.observableArray(["Completed", "Cannot complete"]),
        notification: {
            message: ko.observable(),
            type: ko.observable(),
            exists: ko.observable(false),
        },
        handleResponse: function (response, which, message) {
            var that = this;
            that.notification.message(message);
            that.notification.type(which);
            that.notification.exists(true);
        },
        tasks: ko.observableArray(),
        expand: function (actId, data, event) {
            var that = this;
            var el = "#activity-details-" + actId;
            $(el).toggle();
            var chevron = ($(el).is(":visible")) ? "fa fa-chevron-up" : "fa fa-chevron-down";
            $(event.currentTarget).children("span").removeClass().addClass(chevron);
            that.unload($(el));
        },
        unload: function (el) {
            var that = this;
            $(".collapsible-panel-body").not($(el)).hide();
            $(".collapsible-panel-body").not($(el)).prev().children("span").removeClass().addClass("fa fa-chevron-down");
            that.tasks([]);
            that.tasksLoaded(false);
        },
        unvolunteer: function (activityId) {
            var that = this;
            $.ajax({
                url: "/api/activity/" + activityId + "/signup",
                type: "DELETE",
                contentType: "application/json"
            }).done(function (response) {
                window.location = window.location;
            }).fail(function (response) {
                that.handleResponse(response, "error", "An error has happened");
            });
        },
        loadTasks: function (activityId, data, event) {
            var that = this;
            $.get("/MyActivities/" + activityId + "/tasks")
                .done(function (response) {
                if (response.length < 1) {
                    that.handleResponse(response, 'notification', 'No results');
                    return;
                }
                var temp = [];
                for (var i = 0; i < response.length; i++) {
                    temp.push(new TaskModel(response[i].Status, response[i].StatusDescription, response[i].Id, response[i].TaskName, response[i].TaskId));
                }
                that.tasks(temp);
                that.tasksLoaded(true);
            }).fail(function (response) {
                that.handleResponse(response, "error", "An error has happened");
            });
        },
        updateTasks: function (actId) {
            var that = this;
            var url = "/MyActivities/" + actId + "/tasks";
            var sendData = [];
            for (var i = 0; i < that.tasks().length; i++) {
                var current = that.tasks()[i];
                sendData.push({
                    Id: current.id(),
                    TaskName: current.name(),
                    Status: current.status(),
                    StatusDescription: current.statusDescription(),
                    TaskId: current.taskId()
                });
            }
            $.ajax({
                url: url,
                type: "POST",
                data: JSON.stringify(sendData),
                contentType: "application/json"
            }).done(function (response) {
                console.log(response);
                if (response.success) {
                    that.handleResponse(response, "success", "Update completed");
                }
            }).fail(function (response) {
                that.handleResponse(response, "error", "An error has happened");
            });
        }
    };
    ko.applyBindings(myActivitiesViewModel);
})(ko, $);
