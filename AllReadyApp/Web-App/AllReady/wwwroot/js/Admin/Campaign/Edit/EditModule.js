/// <reference path="../../../../lib/jquery/dist/jquery.js" />
"use strict";

var editModule = (function () {

    function deleteCampaignImageButtonClick() {

        function deleteCampaignImageCallBack(e) {
            e.preventDefault();

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

                        $(".image-delete-success-alert").slideDown(2000, function() {
                            $(this).slideUp(3000);
                        });
                    }

                },
                error: function (xhr, status, error) {
                    //console.log(xhr.responseText);
                    //alert("message : \n" + "An error occurred, for more info check the js console" + "\n status : \n" + status + " \n error : \n" + error);
                }
            });

        }

        $("#delete-image").on("click", deleteCampaignImageCallBack);
    }

    return {
        deleteCampaignImage: deleteCampaignImageButtonClick
    };

})();