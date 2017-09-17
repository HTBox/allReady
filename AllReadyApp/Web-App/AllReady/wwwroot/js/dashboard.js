///<reference path="../lib/jquery/dist/jquery.js" />
(function ($) {
    $('#calendar').fullCalendar({
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

$(document).ready(function () {

    $('#VolunteerTable').dataTable({
        dom: 'Bfrtip',
        serverSide: false,
        processing: true,
        ajax: 
        {
            "type": "GET",
            "url": "../../api/volunteer",
            "contentType": "application/json; charset=utf-8",
            "dataSrc": "" 
        },
        columns: [

               { "data": "WorkflowState" },
               { "data": "Name" },
               { "data": "Location" },
               { "data": "NumberOfTasks" },
               { "data": "NumberOfVolunteers" },
        ],
        select: true,
        buttons: [               
               {
                   extend: 'collection',
                   text: 'Export',
                   buttons: [
                       'copy',
                       'excel',
                       'csv',
                       'pdf',
                       'print'
                   ]
               }
           ]    
    },
    );
}); 