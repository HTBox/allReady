/// <reference path="../../../../lib/jquery/dist/jquery.js" />
"use strict";

define("EditModule", function () {

    function deleteEventImageButtonClickEventHandler() {

        function deleteEventImageCallBack(e) {
            e.preventDefault();

            var confirmAnswer = confirm("Are you sure you want to delete the image?");

            if (!confirmAnswer) return;

            var eventId = $("#Id").val();
            var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                type: "POST",
                url: "/Admin/Event/DeleteEventImage",
                data: { __RequestVerificationToken: antiForgeryToken, eventId: eventId },
                dataType: "json",
                success: function (response) {
                    
                    if (response.status === "Success") {
                        $("#image-panel-container").slideUp(1000);
                        handleAlertAppearanceAndDisAppearance("alert-success", "The image deleted successfully.", ".event-image-status-alert");
                    }

                    if (response.status === "NotFound") {
                        handleAlertAppearanceAndDisAppearance("alert-warning", "There wasn't any event to delete the image from.", ".event-image-status-alert");
                    }

                    if (response.status === "Unauthorized") {
                        handleAlertAppearanceAndDisAppearance("alert-warning", "Insufficient authorization to proceed with this task.", ".event-image-status-alert");
                    }

                    if (response.status === "NothingToDelete") {
                        handleAlertAppearanceAndDisAppearance("alert-warning", "The event doesn't have any image to delete.", ".event-image-status-alert");
                    }

                },
                error: function (xhr, status, error) {
                    //console.log(xhr.responseText);
                    handleAlertAppearanceAndDisAppearance("alert-warning", "An error has occurred, operation was unsuccessful." + "\n status : \n" + status + " \n error : \n" + error, ".event-image-status-alert");
                }
            });

        }

        $("#delete-image").on("click", deleteEventImageCallBack);
    }

    function checkForMobileDeviceAndShowImageDeleteButton() {
        if (/Mobi/.test(navigator.userAgent)) {
            $("#image-wrapper button").show();
        }
    }

    function handleAlertAppearanceAndDisAppearance(alertClass, alertMessage, selector) {

        var alertElemet = $(selector);

        alertElemet.addClass(alertClass);
        alertElemet.text(alertMessage);

        alertElemet.slideDown(1000, function () {
            setTimeout(function () {
                alertElemet.slideUp(1000, function () {
                    alertElemet.removeClass(alertClass);
                });
            }, 3000);
        });
    }

    return {
        addDeleteEventImageHandler: deleteEventImageButtonClickEventHandler,
        checkForMobileDeviceAndShowImageDeleteButton: checkForMobileDeviceAndShowImageDeleteButton
    };

});