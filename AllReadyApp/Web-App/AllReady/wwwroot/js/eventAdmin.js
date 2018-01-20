$(function () {
    // handle the confirm click
    $("#confirmOverwriteLocation").click(function () {
        var id = $("#OrganizationId").val();
        var getContactInfo = $.ajax({ url: "/admin/api/organization/" + id + "/Contact", method: "GET", cache: true });
        getContactInfo.then(function (data) {
                if (data != null) {
                    if (data.Location != null) {
                        $("#Location_Address1").val(data.Location.Address1);
                        $("#Location_Address2").val(data.Location.Address2);
                        $("#Location_City").val(data.Location.City);
                        $("#Location_State").val(data.Location.State);
                        $("#Location_PostalCode").val(data.Location.PostalCode);
                        $("#Location_Country").val(data.Location.Country);
                    }
                }
                $('#confirmLocationModal').modal('hide');
            })
            .fail(function (e, t, m) {
                alert("Ajax Error:" + e);
            });
    });

});
