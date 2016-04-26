﻿var HTBox;
(function (HTBox) {
    var EventDetailAdmin = (function () {
        function EventDetailAdmin() {
            $("#messageVolunteersModal").on("show.bs.modal", function (e) {
                var modal = $(this);
                $('#messageCharacterCount').html("");
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
                        }, 500);
                    },
                    error: function (error) {
                        $(".alert-info", form).hide();
                        var errorSection = $(".alert-danger", form);
                        var errorMessage = "";
                        if (error.responseText) {
                            var errorInfo = JSON.parse(error.responseText);
                            
                            if (errorInfo.Subject && errorInfo.Subject.length > 0) {
                                errorMessage = errorMessage + errorInfo.Subject[0] + "<br/>";
                            }
                            if (errorInfo.Message && errorInfo.Message.length > 0) {
                                errorMessage = errorMessage + errorInfo.Message[0] + "<br/>";
                            }                            
                        }
                        if (errorMessage === "") {
                            errorMessage = "An error occurred while attempting to send message. Please try again.";
                        }
                        errorSection.html(errorMessage);
                        errorSection.show();
                        $("#sendMessageToVolunteers").removeAttr('disabled');
                    }
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
        return EventDetailAdmin;
    })();
    HTBox.EventDetailAdmin = EventDetailAdmin;
})(HTBox || (HTBox = {}));
