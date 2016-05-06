$(function() {
    setEventButtonSize();
    finaliseEventButtonSize();
    $(window).resize(setEventButtonSize);

    $(".event-btn").click(function(e) {
        e.preventDefault();
        startTimer(5000);
    });
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
    timer.text(ms / 1000);

    var id = setInterval(function() {
        ms -= 1000;
        if (ms < 0) {
            clearInterval(id);
            ms = 0;
            onTimerEnd();
        }
        timer.text(ms / 1000);
    }, 1000);
}

function onTimerEnd() {
    var btns = $(".event-btn");
    btns.css("background", "rgba(80,80,80,80)");
    btns.children("p").text("WAITING FOR NEXT VOTE.");
}