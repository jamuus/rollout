var log = console.log;

var spheroManager = require('./spheroManager');
log('Setting up manager');
var manager = spheroManager();


log('Waiting for a sphero to connect');
manager.onSpheroConnect(function(sphero) {
    log('New sphero connected:', manager.names, manager.count);
    sphero.newDataCallback(function(data) {
        // log('New velocity:', data);
    });
    var i = 0;
    setInterval(function() {
        i += 40;
        sphero.force(i, 0.4);
    }, 100);
    // sphero.force(0, 0.1);
});
