'use strict';
var sylvester = require('sylvester');

var Matrix = sylvester.Matrix;


var filters = require('./filters.js');
var KalmanModel = filters.KalmanModel;
var KalmanObservation = filters.KalmanObservation;
var vec2log = filters.vec2log;
var Filter = filters.Filter;

var spheroIds = [
    'ybr',
    'boo',
];

var spheros = {};

function nothing() {
    // console.log('nothing b0ss');
}

var fs = require('fs');
var settings;
process.on('SIGINT', function() {
    console.log('Goodbye!');
});

if (process.argv[2]) {
    var settingsFile = process.argv[2];
    console.log(settingsFile);
    fs.readFile(settingsFile, (err, file) => {
        if (err) {
            console.log('Couldnt open settings file', settingsFile, err);
            return;
        }
        settings = JSON.parse(file.toString());
    });
}

function initSphero(dataOut) {
    // initial state
    var x_0 = $V([0, 0, 0, 0]);

    // initial prediction error (always 1)
    var P_0 = $M([
        [1, 0, 0, 0],
        [0, 1, 0, 0],
        [0, 0, 1, 0],
        [0, 0, 0, 1],
    ]);

    // the input model
    // to add the velocity to position
    var F_k = $M([
        [1, 0, 1 / 60, 0],
        [0, 1, 0, 1 / 60],
        [0, 0, 1, 0],
        [0, 0, 0, 1],
    ]);


    // process noise (wadafaq)
    var Q_k = $M([
        [0.01, 0, 0, 0],
        [0, 0.01, 0, 0],
        [0, 0, 0.01, 0],
        [0, 0, 0, 0.01],
    ]);
    var KM = new KalmanModel(x_0, P_0, F_k, Q_k);


    // observation value
    var z_k = $V([0, 0, 0, 0]);

    // observation model
    var H_k = $M([
        [1, 0, 0, 0],
        [0, 1, 0, 0],
        [0, 0, 0, 0],
        [0, 0, 0, 0],
    ]);

    // noise
    var R_k = $M([
        [1, 0, 0, 0],
        [0, 1, 0, 0],
        [0, 0, 0.1, 0],
        [0, 0, 0, 0.1],
    ]);

    var Kimage = new KalmanObservation(z_k, H_k, R_k);

    H_k = $M([
        [0, 0, 0, 0],
        [0, 0, 0, 0],
        [0, 0, 1, 0],
        [0, 0, 0, 1],
    ]);

    var Ksphero = new KalmanObservation(z_k, H_k, R_k);
    var spheroPos = vec2log(500);
    spheroPos.add({
        x: 0,
        y: 0
    });
    var scaleLog = vec2log(40);
    scaleLog.add({
        x: 0,
        y: 0,
    });
    var ret = {
        ipPos: vec2log(100),
        spheroVel: vec2log(100),
        spheroPos,
        batteryVoltage: -1,
        force: nothing,
        driftAngle: 0,
        absSpheroVel: {
            x: 0,
            y: 0
        },
        pos: {
            x: 0,
            y: 0
        },
        dx: 0,
        dy: 0,
        kalmanModel: KM,
        kSpheroObservation: Ksphero,
        kIPObservation: Kimage,
        angleVecLog: vec2log(20),
        scaleLog,
        scale: 0.2,
        lastCalib: new Date().getTime(),
        lastSpheroData: new Date().getTime(),
        lastIpData: new Date().getTime()
    }
    setInterval(() => {
        KM.update(Ksphero);
        if (new Date().getTime() - ret.lastIpData < 50) {
            KM.update(Kimage);
        }

        var x = KM.x_k.elements[0];
        var y = KM.x_k.elements[1];
        if (x < -20) x = -20;
        if (x > 20) x = 20;
        if (y < -20) y = -20;
        if (y > 20) y = 20;

        ret.pos = {
            x: x,
            y: -y,
        };

        dataOut({
            x: -ret.pos.x,
            y: -ret.pos.y
        });
    }, 1000 / 60);

    return ret;
}


var transformCorners;

var imageSize = {
    x: 1280.0,
    y: 720.0,
};

var spheroScale = 1;
var outputScale = 1;

var debugLog = console.log;
var offset = {
    x: 0,
    y: 0
};

function updateSpheroDrift(sphero, result) {
    if (result.distance < 2000 && result.maxDistance > 5) {
        sphero.angleVecLog.add(result.angleVec);
        sphero.driftAngle = vecAngle(sphero.angleVecLog.average());

        sphero.scaleLog.add({
            x: result.scale,
            y: 0
        });
        sphero.scale = sphero.scaleLog.average().x;

        console.log('Drift: ', sphero.driftAngle, 'dist', result.distance, 'maxDist', result.maxDistance, 'scale', result.scale);
    }
}

module.exports = function(fn, workers) {
    var dataOut = fn.dataOut;

    spheros = {
        "ybr": initSphero(pos => dataOut({
            name: 'ybr',
            data: {
                kalmanPos: pos
            }
        })),
        "boo": initSphero(pos => dataOut({
            name: 'boo',
            data: {
                kalmanPos: pos
            }
        })),
    };

    workers[0].on('message', result => {
        var sphero = spheros[spheroIds[0]];
        updateSpheroDrift(sphero, result);
        // console.log(spheroIds[0], sphero.driftAngle);
        dataOut({
            name: spheroIds[0],
            data: {
                scale: result.scale,
                driftAngle: result.driftAngle
            }
        });
    });
    workers[1].on('message', result => {
        var sphero = spheros[spheroIds[1]];
        updateSpheroDrift(sphero, result);
        // console.log(spheroIds[1], sphero.driftAngle);
        dataOut({
            name: spheroIds[1],
            data: {
                scale: result.scale,
                driftAngle: result.driftAngle
            }
        });
    });

    // called by web client to test forces
    fn.forceCallback(function(data) {
        spheros.ybr.force(data.direction, data.force);
        spheros.boo.force(data.direction, data.force);
    });

    fn.transformCallback(function(data) {
        offset = data.points[0];
    });

    fn.spheroScaleCallback(function(data) {
        console.log('spheroScale', data);
        if (data.spheroScale)
            spheroScale = data.spheroScale;
        if (data.outputScale)
            outputScale = data.outputScale;
    });

    setupSpheroManager(dataOut);
    setupIp(dataOut, workers);

    return spheros;
}

function setupSpheroManager(dataOut) {
    var ipc = require('node-ipc');

    ipc.config.id = 'hello';
    ipc.config.silent = true;

    var id = 'world';

    ipc.connectTo(id, _ => {
        ipc.of[id].on('connect', _ => {
            ipc.log('Connected to sphero manager'.rainbow);
            ipc.of[id].emit('init', '');
        });

        ipc.of[id].on('disconnect', () => {
            ipc.log('disconnected from'.notice);
        });

        ipc.of[id].on('newSpheroData', (data) => {
            var val = JSON.parse(data);
            var name = deviceNameTofriendly(val.name);
            if (val.data.type === 'velocity') {
                dataOut({
                    name,
                    data: newSpheroData(val.data, spheros[name])
                });
            } else {
                ipc.log('Unknown data type'.debug);
            }
        });

        ipc.of[id].on('spheroList', (data) => {
            // console.log('got sphero list', JSON.parse(data));

            var val = JSON.parse(data);
            var deviceName = val[0];
            for (let deviceName of val) {
                var name = deviceNameTofriendly(deviceName);
                var sphero = spheros[name];
                ((sphero) => {
                    sphero.force = (direction, force) => {
                        let offsetDirection = direction + sphero.driftAngle;

                        let send = {
                            name: deviceName,
                            direction: offsetDirection,
                            force
                        };
                        ipc.of[id].emit('force', JSON.stringify(send));
                    };
                })(sphero)
            }

        });
    });
}

function deviceNameTofriendly(name) {
    // ew
    return name.toLowerCase().indexOf("ybr") !== -1 ? "ybr" : "boo";
}


function setupIp(dataOut, workers) {
    var PORT = 1337;
    var HOST = '127.0.0.1';

    var dgram = require('dgram');
    var server = dgram.createSocket('udp4');

    server.on('listening', function() {
        var address = server.address();
        debugLog('Listening for image processing data on', address.address + ":" + address.port);
    });
    var i = 0;
    server.on('message', function(message, remote) {
        var data = {};
        var parts = message.toString().split(',');
        data.id = parts[0];
        // absolute position update
        data.x = parseInt(parts[1]);
        data.y = parseInt(parts[2]);

        var sphName = spheroIds[data.id];
        var spheroState = spheros[sphName];
        if (spheroState) {
            dataOut({
                name: sphName,
                data: newIpData(spheroState, data, workers[data.id])
            });
        }
    });

    server.bind(PORT);
}


var lastPos = [{
    x: 0,
    y: 0
}, {
    x: 0,
    y: 0
}];


function newSpheroData(data, spheroState) {
    var dx = data.dx;
    var dy = data.dy;

    // 
    var posMeters = {
        x: dx / 1000.0,
        y: dy / 1000.0,
    };
    spheroState.spheroVel.add(posMeters);

    var aft = new Date().getTime();
    var diff = (aft - spheroState.lastSpheroData) / 1000;
    spheroState.lastSpheroData = aft;

    var lastSpheroPos = spheroState.spheroPos.lastEntry();

    spheroState.spheroPos.add({
        x: lastSpheroPos[0].x + diff * posMeters.x,
        y: lastSpheroPos[0].y + diff * posMeters.y
    });

    var absVelX = dx * Math.cos(spheroState.driftAngle) -
        dy * Math.sin(spheroState.driftAngle);
    var absVelY = dx * Math.sin(spheroState.driftAngle) +
        dy * Math.cos(spheroState.driftAngle);

    spheroState.absSpheroVel = {
        x: absVelX,
        y: absVelY,
    };

    spheroState.kSpheroObservation.z_k = $V(
        [0, 0,
            absVelX * 1 / spheroState.scale * spheroScale,
            absVelY * 1 / spheroState.scale * spheroScale
        ]
    );
    return {
        relSpheroVec: spheroState.spheroVel.average(),
        absSpheroVec: spheroState.absSpheroVel,
    }
}

function tryFitPaths(ipPos1, ipPos2, spheroPos1, spheroPos2, ipData, spheroData) {
    spheroPos1 = spheroPos1[0], spheroPos2 = spheroPos2[0], ipPos1 = ipPos1[0], ipPos2 = ipPos2[0];

    var sphero1Diff = {
        x: spheroPos1.x - 0,
        y: spheroPos1.y - 0
    };

    var translateds1 = {
        x: spheroPos1.x - sphero1Diff.x,
        y: spheroPos1.y - sphero1Diff.y,
    }
    var translateds2 = {
        x: spheroPos2.x - sphero1Diff.x,
        y: spheroPos2.y - sphero1Diff.y,
    };


    var ip1Diff = {
        x: ipPos1.x - 0,
        y: ipPos1.y - 0
    };

    var translatedi1 = {
        x: ipPos1.x - ip1Diff.x,
        y: ipPos1.y - ip1Diff.y
    };

    var translatedi2 = {
        x: ipPos2.x - ip1Diff.x,
        y: ipPos2.y - ip1Diff.y
    };

    var ipMag = magnitude(translatedi2);

    var spMag = magnitude(translateds2);

    if (spMag === 0 || ipMag === 0) {
        return;
    }

    var scale = 1 / spMag * ipMag;

    var normaliseds2 = {
        x: translateds2.x * scale,
        y: translateds2.y * scale,
    };

    var s2angle = Math.atan2(normaliseds2.y, normaliseds2.x);
    var i2angle = Math.atan2(translatedi2.y, translatedi2.x);

    var angleDiff = s2angle - i2angle;
    var vecDiff = {
        x: Math.cos(angleDiff),
        y: Math.sin(angleDiff)
    };

    var t1 = {
        x: -spheroPos1.x,
        y: -spheroPos1.y,
    };

    var transformedSpheroData = transformPath(spheroData, scale, angleDiff, t1, ipPos1);
    // var buf = '';
    // find matching points
    var ipDataMatch = [];
    var spheroDataMatch = [];
    for (var i in ipData.log) {
        var ipe = ipData.log[i];
        var sphe = spheroData.closestEntry(ipe[1]);

        if (Math.abs(ipe[1] - sphe.closest[1]) < 100) {
            ipDataMatch.push(ipe);
            var t1 = transformedSpheroData[sphe.index];
            spheroDataMatch.push(t1);

            // buf += ipe[0].x + '\t' + ipe[0].y + '\t' + t1.x + '\t' + t1.y + '\n';
        }
    }

    var pathDistance = distBetweenPaths(ipDataMatch, spheroDataMatch);
    var maxD = pathSparseness(ipDataMatch);

    return {
        angle: angleDiff,
        angleVec: vecDiff,
        scale,
        distance: pathDistance,
        numMatches: spheroDataMatch.length,
        maxDistance: maxD
    }
}

function distBetweenTwoPoints(a, b) {
    return Math.sqrt(a.x * b.x + a.y * b.y);
}

function pathSparseness(path) {
    var maxD = 0;

    for (var i = 1; i < path.length; i++) {
        var pos = path[i][0];
        // console.log(pos);
        var d = distance(pos, path[0][0]);
        if (d > maxD) {
            maxD = d;
        }
    }
    return maxD;
}

/**
 * Returns a random integer between min (inclusive) and max (inclusive)
 * Using Math.round() will give you a non-uniform distribution!
 */
function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}


var ipDelay = 30;

module.exports.calcSpheroTransform = calcSpheroTransform;

function calcSpheroTransform(_ipData, _spheroData) {
    var ipData = vec2log(_ipData.log.length, _ipData.log);
    var spheroData = vec2log(_spheroData.log.length, _spheroData.log);

    var best = {
        distance: 1e9,
        angle: -1,
        scale: -1
    };
    for (var i = 0; i < 200; i++) {
        var numEntries = ipData.log.length;
        var i1, i2;
        do {
            i1 = getRandomInt(0, numEntries - 1);
            i2 = getRandomInt(0, numEntries - 1);
        } while (i1 === i2 && numEntries > 1);
        var ipPos1 = ipData.log[i1];
        var ipPos2 = ipData.log[i2];
        if (ipPos1 && ipPos2) {
            var spheroPos1 = spheroData.closestEntry(ipPos1[1] - ipDelay);
            var spheroPos2 = spheroData.closestEntry(ipPos2[1] - ipDelay);

            // check if matching points dt isnt too large
            var pdiff1 = Math.abs(ipPos1[1] - spheroPos1.closest[1]);
            var pdiff2 = Math.abs(ipPos2[1] - spheroPos2.closest[1]);
            if (pdiff1 > 100 || pdiff2 > 100) {
                // time difference between the matching sphero data is too big
                // this is the case when not enough sphero data is logged
                continue;
            }


            var path = tryFitPaths(ipPos1, ipPos2, spheroPos1.closest, spheroPos2.closest, ipData, spheroData);
            if (path) {
                var distance = path.distance;

                if (distance < best.distance) {
                    best = path;
                }
            }
        }
    }
    return best;
}

function distBetweenPaths(path1, path2) {
    var acc = 0;
    for (var i in path1) {
        acc += Math.pow(distance(path1[i][0], path2[i]), 2);
    }
    return acc;
}

function distance(a, b) {
    var t = {
        x: b.x - a.x,
        y: b.y - a.y
    };
    return Math.sqrt(t.x * t.x + t.y * t.y);
}

function transformPath(path, scale, angle, t1, t2) {
    var result = [];
    var rotate = Matrix.Rotation(angle);

    for (var i in path.log) {
        var e = path.log[i][0];
        var transl = {
            x: (e.x + t1.x) * scale,
            y: (e.y + t1.y) * scale
        };

        var rotatedPoint = rotate.x($V([transl.x, transl.y]));
        result.push({
            x: rotatedPoint.elements[0] + t2.x,
            y: rotatedPoint.elements[1] + t2.y
        });
    }
    return result;
}

function magnitude(p) {
    return Math.sqrt(p.x * p.x + p.y * p.y);
}

var be = new Date().getTime();

function vecAngle(v) {
    return Math.atan2(v.y, v.x);
}

function newIpData(sphero, data, worker) {
    // data.x, data.y, data.id
    var angle, dx, dy;


    // lost sphero
    if (data.x === -1 || data.y === -1) {
        return {};
    }

    sphero.lastIpData = new Date().getTime();
    var ipData = {
        x: data.x,
        y: data.y,
    }

    var transformedPosition = pixelToPosition(ipData);
    // console.log(transformedPosition);

    sphero.kIPObservation.z_k = $V([transformedPosition.x, transformedPosition.y, 0, 0]);

    sphero.ipPos.add(transformedPosition);

    var aft = new Date().getTime();
    var dif = aft - sphero.lastCalib;
    if (dif > 500) {
        sphero.lastCalib = aft;
        worker.send([sphero.ipPos, sphero.spheroPos]);
    }

    var bottomLeft = pixelToPosition({
            x: 0,
            y: 0
        }),
        topRight = pixelToPosition({
            x: 1280,
            y: 720
        }),
        topLeft = pixelToPosition({
            x: 0,
            y: 720
        }),
        bottomRight = pixelToPosition({
            x: 1280,
            y: 0
        });

    return {
        // driftAngle: sphero.driftAngle ? sphero.driftAngle : 0.0,
        ipPosTransformed: transformedPosition,
        cameraBounds: {
            bottomLeft,
            topRight,
            topLeft,
            bottomRight
        }
    }
}

var angleOffset = 0 / 360 * Math.PI * 2; // camera angle offset
var h = 5; // meters high
var verticalFov = 40 / 360 * Math.PI * 2; // 60 degrees in radiuns
var radiunsPerYPixel = verticalFov / imageSize.y;

var focalLength = 200; // 10 mm?

var resetAngle = angleOffset + verticalFov / 2;

var rotatu = $M([
    [1, 0, 0],
    [0, Math.cos(resetAngle), -Math.sin(resetAngle)],
    [0, Math.sin(resetAngle), Math.cos(resetAngle)]
]);


function pixelToPosition(pos) {
    // make bottom left (0,0)
    var y = imageSize.y - pos.y;

    var x = pos.x - imageSize.x / 2;
    var Z = h / Math.cos(angleOffset + radiunsPerYPixel * y);
    var X = x / (focalLength / Z);
    var Y = y / (focalLength / Z);
    var res = rotatu.x($V([X, Y, Z]));
    // return {
    //     x: pos.x / (imageSize.x * 0.5) - 1.0,
    //     y: (imageSize.y - pos.y) / (imageSize.y * 0.5) - 1.0,
    // }
    return {
        x: res.elements[0] * outputScale + offset.x,
        y: res.elements[1] * outputScale + offset.y,
        z: res.elements[2] * outputScale
    }
}