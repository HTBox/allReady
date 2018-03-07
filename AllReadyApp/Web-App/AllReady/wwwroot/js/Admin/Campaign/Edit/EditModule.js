/// <reference path="../../../../lib/jquery/dist/jquery.js" />
"use strict";

define("EditModule", function () {

    function deleteCampaignImageButtonClickEventHandler() {

        function deleteCampaignImageCallBack(e) {
            e.preventDefault();

            var confirmAnswer = confirm("Are you sure you want to delete the image?");

            if (!confirmAnswer) return;

            var campaignId = $("#Id").val();
            var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                type: "POST",
                url: "/Admin/Campaign/DeleteCampaignImage",
                data: { __RequestVerificationToken: antiForgeryToken, campaignId: campaignId },
                dataType: "json",
                success: function (response) {

                    if (response.status === "Success") {
                        $("#image-panel-container").slideUp(1000);
                        $("#image-url-input").remove();
                        handleAlertAppearanceAndDisAppearance("alert-success", "The image deleted successfully.", ".campaign-image-status-alert");
                    }

                    if (response.status === "NotFound") {
                        handleAlertAppearanceAndDisAppearance("alert-warning", "There wasn't any campaign to delete the image from.", ".campaign-image-status-alert");
                    }

                    if (response.status === "Unauthorized") {
                        handleAlertAppearanceAndDisAppearance("alert-warning", "Insufficient authorization to proceed with this task.", ".campaign-image-status-alert");
                    }

                    if (response.status === "DateInvalid") {
                        handleAlertAppearanceAndDisAppearance("alert-warning", response.message, ".campaign-image-status-alert");
                    }

                    if (response.status === "NothingToDelete") {
                        handleAlertAppearanceAndDisAppearance("alert-warning", "The campaign doesn't have any image to delete.", ".campaign-image-status-alert");
                    }

                },
                error: function (xhr, status, error) {
                    //console.log(xhr.responseText);
                    handleAlertAppearanceAndDisAppearance("alert-warning", "An error has occurred, operation was unsuccessful." + "\n status : \n" + status + " \n error : \n" + error, ".campaign-image-status-alert");
                }
            });

        }

        $("#delete-image").on("click", deleteCampaignImageCallBack);
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

    function populatePreviewTableOnTabClickEventHandler() {

        $("a[href='#preview-tab']").on('show.bs.tab', function (e) {

            var $organizationName = $("#OrganizationId :selected").text();
            $("#organization-name-input").val($organizationName);
            var $formData = $("#campaign-form").serialize();

            $.post("/Admin/Campaign/CampaignPreview", $formData, function (response, textStatus, jqXHR) {
                $("#preview-table-container").html(response);
                $("#full-description-td").html(tinyMCE.activeEditor.getContent({ format: 'raw' }));
            }, "HTML");
        });
    }

    return {
        addDeleteCampaignImageHandler: deleteCampaignImageButtonClickEventHandler,
        checkForMobileDeviceAndShowImageDeleteButton: checkForMobileDeviceAndShowImageDeleteButton,
        addPopulatePreviewTableHandler: populatePreviewTableOnTabClickEventHandler
    };

});
