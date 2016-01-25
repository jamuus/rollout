let debug = true;

module.exports = function() {
    let _onSpheroConnect = () => {};
    let api = {
        count: 0,
        names: [],
        instaces: [],
        onSpheroConnect: callback => {
            _onSpheroConnect = callback;
        }
    };

    // todo: parameterise
    let updatePerSecond = 10;
    let dataPerSecond = 30;

    let sphero = require('sphero');
    let fs = require('fs');
    let log = function() {
        if (debug)
            console.log.apply(this, arguments);
    }

    let connectedSpheros = {
            instances: [],
            deviceNames: [],
            friendlyNames: []
        },
        desiredSpheros = 1;
    let spheroDeviceRegex = /tty\.Sphero.*/;

    function updateSpheros() {
        if (connectedSpheros.instances.length < desiredSpheros) {
            fs.readdir('/dev/', (err, files) => {
                if (err)
                    return log('[ERROR] in readdir:', err);
                log('Searching for spheros...');
                let unconnectedSpheros = files.filter(device => {
                    return device.match(spheroDeviceRegex);
                }).filter(spheroDevice => {
                    return connectedSpheros.deviceNames.indexOf(spheroDevice) === -1;
                });
                if (unconnectedSpheros.length === 0)
                    log('None found');

                for (let i in unconnectedSpheros) {
                    let newSpheroDev = unconnectedSpheros[i];
                    let spheroInstance = sphero('/dev/' + newSpheroDev);

                    let spheroConnectCallback = ((instance, deviceName) => {
                        return (err) => {
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
                        }
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
            api.instaces.splice(i);
        }
    }

    function setupSpheroInstance(sphero, deviceName) {
        let _newDataCallback = () => {};
        let spheroForce = {
            direction: 0,
            power: 0
        };
        setInterval(() => {
            sphero.roll(Math.round(spheroForce.power * 255),
                spheroForce.direction % 360)
        }, 1000 / updatePerSecond);
        api.names.push(deviceName);
        api.count++;
        api.instances.push({
            name: deviceName,
            newDataCallback: callback => {
                _newDataCallback = callback;
            },
            force: (direction, power) => {
                if (power > 1)
                    power = 1;
                if (power < 0)
                    power = 0;
                spheroForce.direction = direction;
                spheroForce.power = power;
            }
        });

        sphero.streamVelocity(dataPerSecond);
        sphero.on("velocity", _data => {
            // parse data
            let data = {
                xVelocity: _data.xVelocity.value[0],
                yVelocity: _data.yVelocity.value[0],
            }
            _newDataCallback(data);
        });

        sphero.on('error', (err) => {
            log('[ERROR] in sphero', deviceName, '-', err);
        });

        sphero.on('close', () => {
            log('Connection to', deviceName, 'closed');
            removeSphero(sphero, deviceName);
            updateSpheros();
        });

        _onSpheroConnect(api[deviceName]);
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
