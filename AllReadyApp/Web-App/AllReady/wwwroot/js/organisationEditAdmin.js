(function() {
    'use strict';
    $('#pp-url').hide();
    $('#pp-text').hide();

    $('#show-pp-url').click(function(event) {
        event.preventDefault();
        $('#pp-text').slideUp();
        $('#pp-url').slideDown();
    });
    $('#show-pp-text').click(function(event) {
        event.preventDefault();
        $('#pp-url').slideUp();
        $('#pp-text').slideDown();
    })
}());