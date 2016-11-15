///<reference path="../lib/jquery/dist/jquery.js" />
(function ($) {
    $('#calendarr').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        defaultView: 'month',

        eventSources: [{

            events: function (star, end, timezone, callback) {

                $.get("../../api/Event/" + star.format('YYYY-MM-DD') + "/" + end.format('YYYY-MM-DD'), function (result) {
                    var events = [];
                    for (var i = 0; i < result.length; i++) {
                        var event = result[i];
                        events.push({
                            title: event.Title,
                            start: event.StartDateTime,
                            end: event.EndDateTime,
                            url: "/Event/" + event.Id
                        });
                    }
                    callback(events);

                });
            },
            color: "#d58033"
        }
        ]
    });

})($);