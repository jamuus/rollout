'use strict';

var debug = true;

module.exports = function(opts) {
    opts = opts || {};
    var _onSpheroConnect = function _onSpheroConnect() {};
    var api = {
        instances: [],
        onSpheroConnect: function onSpheroConnect(callback) {
            _onSpheroConnect = callback;
        }
    };

    var updatePerSecond = 1;
    var dataPerSecond = opts.dataPerSecond || 20;

    var sphero = require('sphero');
    var fs = require('fs');
    var log = function log() {
        if (debug) console.log.apply(this, arguments);
    };

    var connectedSpheros = {
        deviceNames: [],
    };
    var spheroDeviceRegex = /tty\.Sphero.*/;

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
                var sphDevName = newSph[0]
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
                    }
                    setTimeout(updateSpheros, 2000);
                });
            } else {
                // console.log('[SPHERO] No spheros found');
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
            // if (newpower !== 0) {
            // log('rolling', newpower, newangle);
            if (connected) {
                sphero.roll(newpower, newangle, doRoll);
            }
        }
        doRoll();
        var inst = {
            name: deviceName,
            newDataCallback: function newDataCallback(callback) {
                _newDataCallback = callback;
            },
            force: function force(direction, power) {
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
                dx: _data.xVelocity.value[0],
                dy: _data.yVelocity.value[0]
            };
            _newDataCallback(data, 'velocity');
        });

        sphero.on('error', function(err) {
            log('[ERROR] in sphero', deviceName, '-', err);
            removeSphero(sphero, deviceName);
            sphero.disconnect();
        });

        sphero.on('close', function() {
            log('Connection to', deviceName, 'closed');
            removeSphero(sphero, deviceName);
            connected = false;
        });

        sphero.setRgbLed({
            red: 0,
            green: 0,
            blue: 0
        });

        // var angle = 0;
        // setInterval(() => {
        //     angle += (Math.PI / 30) / 2;
        //     inst.force(angle, 0.2);
        // }, 1000 / 60);


        _onSpheroConnect(api.instances[api.instances.length - 1]);
    }

    updateSpheros();

    return api;
};
