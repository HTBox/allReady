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

            $("#name-td").text($("#Name").val());
            $("#description-td").text($("#Description").val());
            $("#headline-td").text($("#Headline").val());
            $("#full-description-td").html(tinyMCE.activeEditor.getContent({ format: 'raw' }));
            $("#external-url-td").text($("#ExternalUrl").val());
            $("#external-url-text-td").text($("#ExternalUrlText").val());
            $("#timezoneid-td").text($("#TimeZoneId").val());
            $("#start-date-td").text($("#StartDate").val());
            $("#end-date-td").text($("#EndDate").val());
            $("#organizationid-td").text($("#OrganizationId :selected").text());
            $("#featured-td").text($("#Featured").is(":checked") === true ? "Yes" : "No");


            if ($("#image-wrapper").length !== 0) {
                var cloneImage = $("#image-wrapper").closest(".form-group").clone();
                cloneImage.find("button").remove();
                cloneImage.find("label").remove();
                $("#image-url-td").html(cloneImage);
            }



            $("#address1-td").text($("#Location_Address1").val());
            $("#address2-td").text($("#Location_Address2").val());
            $("#city-td").text($("#Location_City").val());
            $("#state-td").text($("#Location_State").val());
            $("#postalCode-td").text($("#Location_PostalCode").val());
            $("#country-td").text($("#Location_Country").val());




            $("#primary-contact-firstname-td").text($("#PrimaryContactFirstName").val());
            $("#primary-contact-lastname-td").text($("#PrimaryContactLastName").val());
            $("#primary-contact-phone-number-td").text($("#PrimaryContactPhoneNumber").val());
            $("#primary-contact-email-td").text($("#PrimaryContactEmail").val());




            if ($("#is-edit").val() === "True") {

                $("#display-impact-goal-td").text($("#CampaignImpact_Display").is(":checked") === true ? "Yes" : "No");
                $("#campaign-impact-goal-td").text($("#CampaignImpact_TextualImpactGoal").val());

                $("#impact-type-td").text($("#CampaignImpact_ImpactType :selected").text());
                var currentImpactLevelTd = $("#current-impact-level-td");
                var numericImpactGoalTd = $("#numeric-impact-goal-td");
                currentImpactLevelTd.closest("tr").hide();
                numericImpactGoalTd.closest("tr").hide();

                if ($("#CampaignImpact_ImpactType").val() === "0") {
                    currentImpactLevelTd.closest("tr").show();
                    numericImpactGoalTd.closest("tr").show();
                    currentImpactLevelTd.text($("#CampaignImpact_CurrentImpactLevel").val());
                    numericImpactGoalTd.text($("#CampaignImpact_NumericImpactGoal").val());
                }
            }

        });

    }

    return {
        addDeleteCampaignImageHandler: deleteCampaignImageButtonClickEventHandler,
        checkForMobileDeviceAndShowImageDeleteButton: checkForMobileDeviceAndShowImageDeleteButton,
        addPopulatePreviewTableHandler: populatePreviewTableOnTabClickEventHandler
    };

});