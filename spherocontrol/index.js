let sphero = require('./spheron/lib');

let fs = require('fs');
let log = console.log;

let connectedSpheros = {
        instances: [],
        deviceNames: []
    },
    desiredSpheros = 1;

let spheroDeviceRegex = /cu\.Sphero.*/

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
                            connectedSpheros.instances.push(instance);
                            connectedSpheros.deviceNames.push(deviceName);
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
    connectedSpheros.instances.splice(sphero);
    connectedSpheros.deviceNames.splice(deviceName);
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

    sphero.setDataStreaming((err, data) => {
        log('Data from sphero', err, data);
    });
}

updateSpheros();

// let i = 0;

// setInterval(() => {
//     connectedSpheros.instances.forEach((sphero) => {
//         i = (i + 1) % 255;
//         sphero.setRGB(0x000000 + i);
//     });
// }, 1000 / 60);
