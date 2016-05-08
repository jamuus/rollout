var lastEventTime = new Date().getTime();

$(function() {
    setEventButtonSize();
    finaliseEventButtonSize();
    $(window).resize(setEventButtonSize);

    //$(".event-btn").click(eventButtonClick);
    
    onTimerEnd();
    setInterval(pollEvents, 500);
});

function setEventButtonSize() {
    var btns = $(".event-btn");
    btns.css("height", btns.css("width"));
    btns.css("line-height", btns.css("height"));
}

function finaliseEventButtonSize() {
    var btns = $(".event-btn");
    btns.css("max-width", btns.css("width"));
}

function startTimer(ms) {
    var timer = $("#event-timer > p");
    timer.text(Math.floor(ms / 1000));

    var id = setInterval(function() {
        ms -= 1000;
        if (ms < 0) {
            clearInterval(id);
            ms = 0;
            onTimerEnd();
        }
        timer.text(Math.floor(ms / 1000));
    }, 1000);
}

function onTimerEnd() {
    var btns = $(".event-btn");
    btns.css("background", "rgba(80,80,80,80)");
    btns.children("p").text("WAITING FOR NEXT VOTE.");
}

function eventButtonClick(e) {
    e.preventDefault();
    //startTimer(5000);
    
    $.post("/webapp/vote", e.target.id);
    $(".event-btn").unbind("click");
}

function pollEvents() {
    $.get("/webapp/events", "", function(data) {
        var obj = JSON.parse(data);
        
        // If our time < event time, new events.
        if (lastEventTime < obj.time)
            setEvents(obj);
    });
}

function setEvents(json) {
    lastEventTime = json.time;
    $("#event-btn-left").text(getEventNameById(json.event0));
    $("#event-btn-right").text(getEventNameById(json.event1));
    $(".event-btn").click(eventButtonClick);
    startTimer(10000 - (new Date().getTime() - lastEventTime));
}

function getEventNameById(id) {
    console.log("EVENT " + id);
    switch (id) {
        case 0:
            return "Hell";
        case 1:
            return "Earthquake";
        case 2:
            return "Enemy";
        case 3:
            return "Weapons";
        default:
            return "Unknown";
    }
}