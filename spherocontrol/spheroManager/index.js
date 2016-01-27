'use strict';

var debug = true;

module.exports = function(opts) {
    opts = opts || {};
    var _onSpheroConnect = function _onSpheroConnect() {};
    var api = {
        count: 0,
        names: [],
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
            instances: [],
            deviceNames: [],
            friendlyNames: []
        },
        desiredSpheros = 1;
    var spheroDeviceRegex = /tty\.Sphero.*/;

    function updateSpheros() {
        if (connectedSpheros.instances.length < desiredSpheros) {
            fs.readdir('/dev/', function(err, files) {
                if (err) return log('[ERROR] in readdir:', err);
                log('Searching for spheros...');
                var unconnectedSpheros = files.filter(function(device) {
                    return device.match(spheroDeviceRegex);
                }).filter(function(spheroDevice) {
                    return connectedSpheros.deviceNames.indexOf(spheroDevice) === -1;
                });
                if (unconnectedSpheros.length === 0) log('None found');

                for (var i in unconnectedSpheros) {
                    var newSpheroDev = unconnectedSpheros[i];
                    var spheroInstance = sphero('/dev/' + newSpheroDev);

                    var spheroConnectCallback = (function(instance, deviceName) {
                        return function(err) {
                            if (err) {
                                log('[ERROR] in spheroOpen', err);
                                log('        Trying again in 1 second');
                                setTimeout(updateSpheros, 1000);
                            } else {
                                log('Succesfully connected', deviceName);
                                setupSpheroInstance(instance, deviceName);
                                connectedSpheros.instances.push(instance);
                                connectedSpheros.deviceNames.push(deviceName);
                            }
                        };
                    })(spheroInstance, newSpheroDev);
                    log('Connecting to', newSpheroDev);
                    spheroInstance.connect(spheroConnectCallback);
                }
            });
        }
    }

    function removeSphero(sphero, deviceName) {
        connectedSpheros.instances.splice(sphero);
        connectedSpheros.deviceNames.splice(deviceName);
        api.names.splice(deviceName);
        var i = api.instances.indexOf(sphero);
        if (i !== -1) {
            api.instances.splice(i);
        }
        clearInterval(sphero.powerStateInterval);
    }

    function setupSpheroInstance(sphero, deviceName) {
        var _newDataCallback = function _newDataCallback() {};
        var spheroForce = {
            direction: 0,
            power: 0
        };

        function doRoll() {
            var newpower = Math.round(spheroForce.power * 255);
            var newangle = (spheroForce.direction / (2 * Math.PI) * 360) % 360;
            if (newpower !== 0) {
                log('rolling', newpower);
                sphero.roll(newpower, newangle, function() {
                    doRoll();
                });
            } else {
                // log(newpower);
                setTimeout(doRoll, 100);
            }
        }
        doRoll();
        api.names.push(deviceName);
        api.count++;
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
        // sphero.setStabilization(1, function(err) {});
        sphero.streamVelocity(dataPerSecond);
        sphero.on('velocity', function(_data) {
            var data = {
                dx: _data.xVelocity.value[0],
                dy: _data.yVelocity.value[0]
            };
            _newDataCallback(data, 'velocity');
        });
        // sphero.streamOdometer(dataPerSecond);

        // sphero.on("odometer", function(data) {
        //     _newDataCallback({
        //         x: data.xOdometer.value[0],
        //         y: data.yOdometer.value[0],
        //     });
        // });

        sphero.on('error', function(err) {
            log('[ERROR] in sphero', deviceName, '-', err);
            removeSphero(sphero, deviceName);
            sphero.disconnect();
        });

        sphero.on('close', function() {
            log('Connection to', deviceName, 'closed');
            removeSphero(sphero, deviceName);
            updateSpheros();
        });
        // inst.powerStateInterval = setInterval(function() {
        //     sphero.getPowerState(function(err, data) {
        //         if (err) {
        //             log("[ERROR] in getPowerState:", err);
        //             return;
        //         }

        //         _newDataCallback({
        //             batteryVoltage: data.batteryVoltage
        //         }, 'battery');
        //     });
        // }, 1000);


        _onSpheroConnect(api.instances[api.instances.length - 1]);
    }

    updateSpheros();

    return api;
};

// var spheros = require('./spheros');
// // number of spheros
// spheros.count;
// // their names
// var names = spheros.names;
// // get one of the spheros
// var ybr = spheros[names[0]]
// // subscribe to sphero data
// ybr.newDataCallback(function(data) {
//     console.log(data);
// });
// // send sphero north with half force
// ybr.force("north", 0.5);
