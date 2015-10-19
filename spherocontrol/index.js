let sphero = require('sphero');



let fs = require('fs');
let log = console.log;

// let orb = sphero('/dev/tty.Sphero-YBR-AMP-SPP');
// orb.on('error', data => {
//     log(data);
// });

// log('connecting');
// orb.connect(a => {
//     log('connected');
// });

let connectedSpheros = {
        instances: [],
        deviceNames: []
    },
    desiredSpheros = 1;

let spheroDeviceRegex = /tty\.Sphero.*/

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
                let spheroInstance = sphero('/dev/' + newSpheroDev);
                spheroInstance.on('error', err => {
                    log('[ERROR] in spheroOpen', err);
                    log('        Trying again in 1 second');
                    setTimeout(updateSpheros, 1000);
                });

                let spheroConnectCallback = ((instance, deviceName) => {
                    return (err) => {
                        log('Succesfully connected', deviceName);
                        setupSpheroInstance(instance, deviceName);
                        connectedSpheros.instances.push(instance);
                        connectedSpheros.deviceNames.push(deviceName);
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
    // other events include:
    //      ready, open, async, response, data

}

updateSpheros();

// let i = 0;

// setInterval(() => {
//     connectedSpheros.instances.forEach((sphero) => {
//         i = (i + 1) % 255;
//         sphero.setRGB(0x000000 + i);
//     });
// }, 1000 / 60);
