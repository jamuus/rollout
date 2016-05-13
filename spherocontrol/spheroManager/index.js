'use strict';

var debug = true;

/**
 * Returns a random integer between min (inclusive) and max (inclusive)
 * Using Math.round() will give you a non-uniform distribution!
 */
function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

module.exports = function(opts) {
    opts = opts || {};

    var ipc = require('node-ipc');

    ipc.config.id = 'world';
    ipc.config.silent = true;

    var _onSpheroConnect = function _onSpheroConnect() {};
    var api = {
        instances: [],
        onSpheroConnect: function onSpheroConnect(callback) {
            _onSpheroConnect = callback;
        }
    };

    var updatePerSecond = 1;
    var dataPerSecond = opts.dataPerSecond || 30;

    var sphero = require('sphero');
    var fs = require('fs');
    var log = function log() {
        if (debug) console.log.apply(this, arguments);
    };
    var connectedSpheros = {
        deviceNames: [],
    };
    var spheroDeviceRegex = /tty\.Sphero.*/;
    var sockets = [];
    ipc.serve(_ => {
        ipc.server.on('init', (data, socket) => {
            sockets.push(socket);
            var response = connectedSpheros.deviceNames;
            ipc.server.emit(socket,
                'spheroList', JSON.stringify(response)
            );
        });

        ipc.server.on('force', (data, socket) => {
            var val = JSON.parse(data);

            // console.log('got force', val);
            // val.name,
            // val.power,
            // val.direction

            // todo make fasterer
            for (let sphero of api.instances) {
                if (sphero.name === val.name) {
                    sphero.force(val.direction, val.force);
                }
            }
        });
    });

    function sendToAllSockets(type, data) {
        for (let socket of sockets) {
            ipc.server.emit(socket,
                type, JSON.stringify(data)
            );
        }
    }

    ipc.server.start();
    // Searches and connects to unconnected spheros
    function updateSpheros() {
        fs.readdir('/dev/', function(err, files) {
            if (err) return log('[ERROR] in readdir:', err);

            var newSph = files.filter((device) => {
                return device.match(spheroDeviceRegex);
            }).filter((sphName) => {
                return connectedSpheros.deviceNames.indexOf(sphName) === -1;
            });
            if (newSph.length > 0) {
                var sphDevName = newSph[getRandomInt(0, newSph.length - 1)];
                log("[SPHERO] Connecting to sphero", sphDevName);
                var sph = sphero('/dev/' + sphDevName, {
                    emitPacketErrors: true
                });
                sph.connect((err) => {
                    if (err) {
                        log("[SPHERO] Error connecting:", err);
                    } else {
                        log("[SPHERO] Connected", sphDevName);
                        setupSpheroInstance(sph, sphDevName);
                        connectedSpheros.deviceNames.push(sphDevName);
                        var response = connectedSpheros.deviceNames;
                        sendToAllSockets('spheroList', response);
                    }
                    setTimeout(updateSpheros, 1000);
                });
            } else {
                setTimeout(updateSpheros, 2000);
            }
        });
    }

    function removeSphero(sphero, deviceName) {
        connectedSpheros.deviceNames.splice(deviceName);
    }

    function setupSpheroInstance(sphero, deviceName) {
        var _newDataCallback = function _newDataCallback() {};
        var spheroForce = {
            direction: 0,
            power: 0
        };
        var connected = true;

        function doRoll() {
            var newpower = Math.round(spheroForce.power * 255);
            var newangle = (((spheroForce.direction / (2 * Math.PI)) * 360) + 360) % 360;

            if (connected) {
                sphero.roll(newpower, newangle, doRoll);
            }
        }
        doRoll();
        var inst = {
            name: deviceName,
            newDataCallback: function(callback) {
                _newDataCallback = callback;
            },
            force: function(direction, power) {
                if (power > 1) power = 1;
                if (power < 0) power = 0;
                spheroForce.direction = direction;
                spheroForce.power = power;
            },
        };
        api.instances.push(inst);

        sphero.streamVelocity(dataPerSecond);
        sphero.on('velocity', function(_data) {
            var data = {
                type: 'velocity',
                dx: _data.xVelocity.value[0],
                dy: _data.yVelocity.value[0]
            };
            var response = {
                name: deviceName,
                data
            };
            sendToAllSockets('newSpheroData', response);
        });

        // setInterval(() => {
        //     sphero.getPowerState(function(err, data) {
        //         //     console.log("  recVer:", data.recVer);
        //         //     console.log("  batteryState:", data.batteryState);
        //         //     console.log("  batteryVoltage:", data.batteryVoltage);
        //         //     console.log("  chargeCount:", data.chargeCount);
        //         //     console.log("  secondsSinceCharge:", data.secondsSinceCharge);
        //         if (err) {
        //             console.log('[ERROR] in sphero', deviceName, 'charging err:', err);
        //         } else {
        //             sendToAllSockets('newSpheroData', {
        //                 name: deviceName,
        //                 data: {
        //                     type: 'battery',
        //                     v: data.batteryVoltage
        //                 }
        //             });
        //         }
        //     });
        // }, 5 * 1000);


        sphero.on('error', function(err) {
            log('[ERROR] in sphero', deviceName, '-', err);
            // removeSphero(sphero, deviceName);
            if (connected) {
                connected = false;
                sphero.disconnect();
            }
        });

        sphero.on('close', function() {
            log('Connection to', deviceName, 'closed');
            removeSphero(sphero, deviceName);
            connected = false;
        });
        // if (deviceName.indexOf('GWO') !== -1) {
        //     sphero.setRgbLed({
        //         red: 255,
        //         green: 0,
        //         blue: 0
        //     });
        // } else {
        sphero.setRgbLed({
            red: 255,
            green: 255,
            blue: 255
        });
        // }

        // _onSpheroConnect(api.instances[api.instances.length - 1]);
    }

    updateSpheros();

    return api;
};

module.exports();