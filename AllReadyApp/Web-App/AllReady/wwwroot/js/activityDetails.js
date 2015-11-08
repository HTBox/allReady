var HTBox;
(function (HTBox) {
    var ActivityDetailAdmin = (function () {
        function ActivityDetailAdmin() {
            $("#messageVolunteersModal").on("show.bs.modal", function (e) {
                var modal = $(this);
                $("#sendMessageToVolunteers").removeAttr('disabled');
                $("#messageVolunteersModal-message").val("");
                $("#messageVolunteersModal-subject").val("");
                $(".alert-info", modal).hide();
                $(".alert-danger", modal).hide();
                $(".alert-success", modal).hide();
            });

            $("#messageVolunteersModal form").submit(function(e){
                e.preventDefault();
                $("#sendMessageToVolunteers").attr('disabled', 'disabled');
                var form = $(this);
                $(".alert-info", form).show();
                $(".alert-success", form).hide();
                $(".alert-error", form).hide();
                $.ajax({
                    type: form.attr('method'),
                    url: form.attr('action'),
                    data: form.serialize(),
                    success: function (data) {
                        $(".alert-info", form).hide();
                        $(".alert-success", form).show();
                        setTimeout(function () {
                            $("#messageVolunteersModal").modal("hide");
                        }, 300);
                    }
                }).error(function (error) {
                    $(".alert-danger", form).hide();
                });

                // prevent submitting again
                return false;
            });

            $('#messageCharacterCount').html("");

            $('#messageVolunteersModal-message').keyup(function () {
                var messageLength = $('#messageVolunteersModal-message').val().length;

                $('#messageCharacterCount').html(messageLength + ' characters');
            });
        }
        return ActivityDetailAdmin;
    })();
    HTBox.ActivityDetailAdmin = ActivityDetailAdmin;
})(HTBox || (HTBox = {}));


