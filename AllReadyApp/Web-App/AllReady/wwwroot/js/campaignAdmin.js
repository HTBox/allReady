$(function() {
    $(document).ready(function () {
        toggleImpactDisplay();
    });
    // handle the confirm click
    $("#confirmOverwriteContact").click(function ()
    {
        var id = $("select[id=TenantId").val();
        var getContactInfo = $.ajax({ url: "/admin/api/tenant/" + id + "/Contact", method: "GET", cache: true });
        getContactInfo.then(function (data) {
            if (data != null) {
                $("#PrimaryContactFirstName").val(data.FirstName);
                $("#PrimaryContactLastName").val(data.LastName);
                $("#PrimaryContactPhoneNumber").val(data.PhoneNumber);
                $("#PrimaryContactEmail").val(data.Email);
                if (data.Location != null) {
                    $("#Location_Address1").val(data.Location.Address1);
                    $("#Location_Address2").val(data.Location.Address2);
                    $("#Location_City").val(data.Location.City);
                    $("#Location_State").val(data.Location.State);
                    $("#Location_PostalCode").val(data.Location.PostalCode);
                    $("#Location_Country").val(data.Location.Country);
                }
            }
                $('#confirmContactModal').modal('hide');
            })
        .fail(function (e, t, m) {
            alert("Ajax Error:" + e);
        });
    });
    $("#CampaignImpact_ImpactType").on("change", function () {
        toggleImpactDisplay();
    });

    function toggleImpactDisplay() {
        console.log($("#CampaignImpact_ImpactType").val());
        if ($("#CampaignImpact_ImpactType").val() === "0") {
            $("#numericImpactSection").show();
        }
        else {
            $("#numericImpactSection").hide();
        }
    }
});