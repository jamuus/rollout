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



        /*
            search for spheros in /dev
            connect to first sphero that isnt connected
        */
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
                var sph = sphero('/dev/' + sphDevName);
                sph.connect((err) => {
                    if (err) {
                        log("[SPHERO] Error connecting:", err);
                    } else {
                        log("[SPHERO] Connected", sphDevName)
                        setupSpheroInstance(sph, sphDevName);
                        connectedSpheros.instances.push(sph);
                        connectedSpheros.deviceNames.push(sphDevName);
                    }
                });
            }
        });
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
            var newpower = Math.round(spheroForce.power * 500);
            var newangle = ((spheroForce.direction / (2 * Math.PI) + Math.PI) * 360) % 360;
            if (newpower !== 0) {
                log('rolling', newpower, newangle);
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

        sphero.on('error', function(err) {
            log('[ERROR] in sphero', deviceName, '-', err);
            removeSphero(sphero, deviceName);
            sphero.disconnect();
        });

        sphero.on('close', function() {
            log('Connection to', deviceName, 'closed');
            removeSphero(sphero, deviceName);
        });

        _onSpheroConnect(api.instances[api.instances.length - 1]);
    }

    setInterval(updateSpheros, 2000);

    return api;
};
