var roundRobot = require('node-sphero');
var sphero = new roundRobot.Sphero();

var guy;

sphero.on('connected', function(ball) {
    console.log('Connected, sending commands.');
    sphero.setDataStreaming([sphero.sensors.imu_pitch], null, null, null, function function_name(data) {
        console.log(data);
    });
    guy = ball;
    setInterval(tick, 1000 / 20);
    // sphero.setDataStreaming()
});
var r = 254,
    g = 0,
    b = 0;
var state = 0;

function updateRainbow() {
    if (state === 0) {
        r--;
        g++;
        if (r === 0)
            state = 1;
    } else if (state === 1) {
        g--;
        b++;
        if (g === 0)
            state = 2;
    } else if (state === 2) {
        b--;
        r++;
        if (b === 0)
            state = 0;
    }
}


function tick() {
    // console.log('Setting colour to (' + r + ', ' + g + ', ' + b + ')');
    updateRainbow();
    guy.setBackLED(1);
    guy.setRGBLED(r, g, b, false);
    // console.log('Setting backLED to', backLED / 100);
    // guy.setBackLED(254);
}
sphero.on('error', function(err) {
    console.log('Sphero error:', err);
});

console.log('Connecting to sphero...');
sphero.connect();
