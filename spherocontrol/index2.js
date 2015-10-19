let sphero = require('./spheron/lib');

let fs = require('fs');
let log = console.log;

let state = {
    connectedSpheros: {
        instances: [],
        deviceNames: []
    },
    desiredSpheros: 1
}


let spheroDeviceRegex = /cu\.Sphero.*/

function updateSpheros() {
    if (state.connectedSpheros.instances.length < state.desiredSpheros) {
        fs.readdir('/dev/', (err, files) => {
            if (err)
                return log('[ERROR] in readdir:', err);
            log('Searching for spheros...');
            let unconnectedSpheros = files.filter(device => {
                return device.match(spheroDeviceRegex);
            }).filter(spheroDevice => {
                return state.connectedSpheros.deviceNames.indexOf(spheroDevice) === -1;
            });
            if (unconnectedSpheros.length === 0)
                log('None found');

            for (let newSpheroDev of unconnectedSpheros) {
                let spheroInstance = sphero.sphero()
                    .resetTimeout(true)
                    .requestAcknowledgement(true);

                let spheroConnectCallback = ((instance, deviceName) => {
                    return (err) => {
                        if (err) {
                            log('[ERROR] in spheroOpen', err);
                            log('        Trying again in 1 second');
                            setTimeout(updateSpheros, 1000);
                        } else {
                            log('Succesfully connected', deviceName);
                            setupSpheroInstance(instance, deviceName);
                            state.connectedSpheros.instances.push(instance);
                            state.connectedSpheros.deviceNames.push(deviceName);
                        }
                    }
                })(spheroInstance, newSpheroDev);
                log('Connecting to', newSpheroDev);
                spheroInstance.open('/dev/' + newSpheroDev, spheroConnectCallback);
            }
        });
    }
}

function removeSphero(sphero, deviceName) {
    state.connectedSpheros.instances.splice(sphero);
    state.connectedSpheros.deviceNames.splice(deviceName);
}

function setupSpheroInstance(sphero, deviceName) {
    sphero.on('error', (err) => {
        log('[ERROR] in sphero', deviceName, '-', err);
        // remove?
    });

    sphero.on('close', () => {
        log('Connection to', deviceName, 'closed');
        removeSphero(sphero, deviceName);
        updateSpheros();
    });

    sphero.on('end', () => {
        log('ended D:');
    });
}

updateSpheros();
