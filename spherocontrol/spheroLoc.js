var debug = console.log;

module.exports = function(spheroManager) {
    var api = {};

    spheroManager.onSpheroConnect(function(newSphero) {
        debug('Sphero', newSphero.name, 'connected.');
        var spheroData = api[newSphero.name] = {
            x: 0,
            y: 0,
            dx: 0,
            dy: 0,
            lastVelocityUpdate: -1,
            batteryVoltage: 0,
            force: newSphero.force,
        };
        newSphero.newDataCallback(function(data, type) {
            for (var dataName in data) {
                api[newSphero.name][dataName] = data[dataName];
            }
            if (type === 'velocity') {
                if (spheroData.lastVelocityUpdate !== -1) {
                    var now = new Date().getTime();
                    var diff = now - spheroData.lastVelocityUpdate;

                    spheroData.x += (diff / 1000) * spheroData.dx / 100;
                    spheroData.y += (diff / 1000) * spheroData.dy / 100;

                    spheroData.lastVelocityUpdate = now;
                } else {
                    spheroData.lastVelocityUpdate = new Date().getTime();
                }
            }
        });
    });

    var PORT = 1337;
    var HOST = '127.0.0.1';

    var dgram = require('dgram');
    var server = dgram.createSocket('udp4');

    server.on('listening', function() {
        var address = server.address();
        debug('UDP Server listening on ' + address.address + ":" + address.port);
    });

    server.on('message', function(message, remote) {
        var data = {};
        var parts = message.toString().split(',');
        data.id = parts[0];
        data.x = parseInt(parts[1]);
        data.y = parseInt(parts[2]);

        // map id to sphero
        // 0 -> ybr -> blue
        // 1 -> boo -> orange

        var idToDev = [
            'tty.Sphero-YBR-AMP-SPP',
            'boo',
        ];
        var sphName = idToDev[data.id];
        if (api[sphName]) {
            api[sphName].x = data.x;
            api[sphName].y = data.y;
        }

    });

    server.bind(PORT, HOST);

    return api;
}
