var maxCount;
$('#assignVolunteersModal .modal-body').on('change', 'input[type="checkbox"]', function (e) {
    var count = $('#assignVolunteersModal .modal-body input[type="checkbox"]:checked').length;    console.log(maxCount);
    if (count >= maxCount) {
        $('#assignVolunteersModal .modal-body input[type="checkbox"]').not(':checked').prop('disabled', true);
    } else {
        $('#assignVolunteersModal .modal-body input[type="checkbox"]').prop('disabled', false);
    }
});
$('[data-name="assignTaskToVolunteer"]').click(function (e) {
    e.preventDefault();

    var url = this.getAttribute('href');
    var taskId = this.getAttribute('data-id');

    $('#assignVolunteersModal .modal-body').html('');
    $.getJSON(url,
        function (data) {
            data.Volunteers.forEach(function (item) {
                $('#assignVolunteersModal .modal-body')
                    .append(
                        '<div class="checkbox"><label><input type="checkbox" name="UserId" value="' +
                        item.Value +
                        '">' +
                        item.Text +
                        '</label></div>');
            });
            $('#assign-TaskId').val(taskId);
            $('#assignVolunteersModal').modal();
            maxCount = data.MaxSelectableCount;
        });
});
