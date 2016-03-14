var sylvester = require('sylvester');
var Matrix = sylvester.Matrix;

var KalmanModel = (function() {

    function KalmanModel(x_0, P_0, F_k, Q_k) {
        this.x_k = x_0;
        this.P_k = P_0;
        this.F_k = F_k;
        this.Q_k = Q_k;
    }

    KalmanModel.prototype.update = function(o) {
        this.I = Matrix.I(this.P_k.rows());
        //init
        this.x_k_ = this.x_k;
        this.P_k_ = this.P_k;

        //Predict
        this.x_k_k_ = this.F_k.x(this.x_k_);
        this.P_k_k_ = this.F_k.x(this.P_k_.x(this.F_k.transpose())).add(this.Q_k);

        //update
        this.y_k = o.z_k.subtract(o.H_k.x(this.x_k_k_)); //observation residual
        this.S_k = o.H_k.x(this.P_k_k_.x(o.H_k.transpose())).add(o.R_k); //residual covariance
        this.K_k = this.P_k_k_.x(o.H_k.transpose().x(this.S_k.inverse())); //Optimal Kalman gain
        this.x_k = this.x_k_k_.add(this.K_k.x(this.y_k));
        this.P_k = this.I.subtract(this.K_k.x(o.H_k)).x(this.P_k_k_);
    }

    return KalmanModel;
})();

KalmanObservation = (function() {

    function KalmanObservation(z_k, H_k, R_k) {
        this.z_k = z_k; //observation
        this.H_k = H_k; //observation model
        this.R_k = R_k; //observation noise covariance
    }

    return KalmanObservation;
})();

var spheroIds = [
    'ybr',
    'boo',
];

var spheros = {
    "ybr": initSphero(),
    "boo": initSphero(),
}

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

function initSphero() {

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
        [1, 0, 1, 0],
        [0, 1, 0, 1],
        [0, 0, 1, 0],
        [0, 0, 0, 1],
    ]);


    // process noise (wadafaq)
    var Q_k = $M([
        [10, 0, 0, 0],
        [0, 10, 0, 0],
        [0, 0, 10, 0],
        [0, 0, 0, 10],
    ]);
    var KM = new KalmanModel(x_0, P_0, F_k, Q_k);


    // observation value
    var z_k = $V([0, 0, 0, 0]);

    // observation model
    var H_k = $M([
        [1, 0, 0, 0],
        [0, 1, 0, 0],
        [0, 0, 1, 0],
        [0, 0, 0, 1],
    ]);

    // noise
    var R_k = $M([
        [1, 0, 0, 0],
        [0, 1, 0, 0],
        [0, 0, 1, 0],
        [0, 0, 0, 1],
    ]);

    var KO = new KalmanObservation(z_k, H_k, R_k);

    return {
        ipPos: vec2log(4),
        spheroVel: vec2log(10),
        batteryVoltage: -1,
        force: nothing,
        driftAngle: 0,
        absSpheroVel: {
            x: -1,
            y: -1
        },
        pos: {
            x: -1,
            y: -1
        },
        dx: 0,
        dy: 0,
        kalmanModel: KM,
        kalmanObservation: KO
    }
}

function vec2log(size) {
    var log = [];

    function add(item) {
        log.push([item, new Date().getTime()]);
        if (log.length > size) {
            log.splice(0, 1);
        }
    }

    function average() {
        var sum = log.reduce(
            (prev, cur) => {
                return {
                    x: cur[0].x + prev.x,
                    y: cur[0].y + prev.y
                };
            }, {
                x: 0,
                y: 0
            }
        );

        return {
            x: sum.x / size,
            y: sum.y / size
        }
    }

    return {
        add,
        average
    };
}

function Filter(size) {
    var log = [];

    return {
        add: function(item) {
            log.push(item);
            if (log.length > size) {
                log.splice(0, 1);
            }
        },
        value: function() {
            return log.reduce((e, i) => e + i, 0) / log.length;
        }
    }
}

function XYFilter(size) {
    var log = [];

    return {
        add: function(item) {
            log.push(item);
            if (log.length > size) {
                log.splice(0, 1);
            }
        },
        value: function() {
            var sum = log.reduce((e, i) => {
                return {
                    x: e.x + i.x,
                    y: e.y + i.y
                }
            }, {
                x: 0,
                y: 0
            });

            return {
                x: sum.x / log.length,
                y: sum.y / log.length
            }
        }
    }
}

var imageTransform;

var debugLog = console.log;

module.exports = function(spheroManager, fn) {
    var dataOut = fn.dataOut;

    // called by web client to test forces
    fn.forceCallback(function(data) {
        spheros.ybr.force(data.direction, data.force);
        spheros.boo.force(data.direction, data.force);
    });

    fn.transformCallback(function(data) {
        imageTransform = data.corners;
    });

    // when a sphero is connected we need to setup some shtuff
    spheroManager.onSpheroConnect(function(newSphero) {
        debugLog('Sphero', newSphero.name, 'connected.');

        var spheroName = newSphero.name.toLowerCase().indexOf("ybr") !== -1 ? "ybr" : "boo";
        var spheroState = spheros[spheroName];

        // assign the api force to the sphero manager force function
        spheroState.force = (dir, force) => {
            var newDir = dir - spheroState.driftAngle;
            newSphero.force(newDir, force);
        }

        // when the sphero sends data we need to update our state
        newSphero.newDataCallback((data, type) => {
            // data is only velocity data so far
            if (type === 'velocity') {
                dataOut(newSpheroData(spheroName, data, spheroState));
            }
        });
    });

    setupIp(dataOut);

    return spheros;
}

function setupIp(dataOut) {
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
            i++;
            // if (i % 5 === 0)
            dataOut(newIpData(sphName, spheroState, data));
        }
    });

    server.bind(PORT);
}

var angleLog = Filter(500);
var lastPos = [{
    x: 0,
    y: 0
}, {
    x: 0,
    y: 0
}];

var bef = new Date().getTime();


function newSpheroData(name, data, spheroState) {
    var dx = data.dx;
    var dy = data.dy;
    spheroState.spheroVel.add({
        x: dx / 1000.0,
        y: dy / 1000.0,
    });

    var absVelX = dx * Math.cos(-spheroState.driftAngle) -
        dy * Math.sin(-spheroState.driftAngle);
    var absVelY = dx * Math.sin(-spheroState.driftAngle) +
        dy * Math.cos(-spheroState.driftAngle);

    spheroState.absSpheroVel = {
        x: absVelX,
        y: absVelY,
    };

    var aft = new Date().getTime();
    // number of seconds past
    var diff = (aft - bef) / 1000.0;
    bef = aft;

    // update pos in meters
    // spheroState.pos.x += (absVelX * diff);
    // spheroState.pos.y += (absVelY * diff);
    // console.log(spheroState.pos);

    spheroState.kalmanObservation.z_k = $V([0, 0, absVelX / (1000 * 15), absVelY / (1000 * 15)]);
    spheroState.kalmanObservation.H_k =
        $M([
            [0, 0, 0, 0],
            [0, 0, 0, 0],
            [0, 0, 1, 0],
            [0, 0, 0, 1],
        ]);


    // $M([
    //     [0, 0, 0, 0],
    //     [0, 0, 0, 0],
    //     [0, 0, 0, 0],
    //     [0, 0, 0, 0],
    // ]);

    spheroState.kalmanModel.update(spheroState.kalmanObservation);

    var pos = {
        x: spheroState.kalmanModel.x_k.elements[0],
        y: spheroState.kalmanModel.x_k.elements[1]
    };
    spheroState.pos = pos;

    // console.log(name);

    return {
        spheroData: {
            name: name,
            data: spheroState.spheroVel.average(),
        },
        kalmanData: {
            name: name,
            d: spheroState.kalmanModel.x_k.elements
        }
    }
}

function newIpData(name, sphero, data) {
    // data.x, data.y, data.id
    var angle, dx, dy;

    // lost sphero
    if (data.x === -1 || data.y === -1) {
        return data.id === 1 ? {
            spheroData: {
                name: name,
                data: sphero.absSpheroVel,
            }
        } : {};
    }

    var ipData = {
        x: data.x,
        y: data.y,
    }

    var transformedPosition = pixelToPosition(ipData);
    // console.log(transformedPosition);

    sphero.kalmanObservation.z_k = $V([transformedPosition.x, transformedPosition.y, 0, 0]);
    sphero.kalmanObservation.H_k =
        $M([
            [1, 0, 0, 0],
            [0, 1, 0, 0],
            [0, 0, 0, 0],
            [0, 0, 0, 0],
        ]);

    // $M([
    //     [0, 0, 0, 0],
    //     [0, 0, 0, 0],
    //     [0, 0, 0, 0],
    //     [0, 0, 0, 0],
    // ]);

    sphero.kalmanModel.update(sphero.kalmanObservation);

    var pos = {
        x: sphero.kalmanModel.x_k.elements[0],
        y: sphero.kalmanModel.x_k.elements[1]
    };
    sphero.pos = pos;

    sphero.ipPos.add(transformedPosition);

    var filteredPos = sphero.ipPos.average();

    ipDx = filteredPos.x - lastPos[data.id].x;
    ipDy = filteredPos.y - lastPos[data.id].y;
    lastPos[data.id] = filteredPos;

    var ipDirVec = {
        x: ipDx,
        y: ipDy
    };

    var avgSpheroData = sphero.spheroVel.average();

    var spheroDirVec = {
        x: avgSpheroData.x,
        y: avgSpheroData.y
    };

    var ipDirMag = Math.sqrt(
        ipDirVec.x * ipDirVec.x +
        ipDirVec.y * ipDirVec.y
    );
    var spheroDirMag = Math.sqrt(
        spheroDirVec.x * spheroDirVec.x +
        spheroDirVec.y * spheroDirVec.y
    );

    var ipDirAngle = Math.atan2(ipDirVec.y, ipDirVec.x);
    spheroDirAngle = Math.atan2(spheroDirVec.y, spheroDirVec.x);
    angle = spheroDirAngle - ipDirAngle;

    angle = angle > Math.PI ? angle - 2 * Math.PI : angle;
    angle = angle < -Math.PI ? angle + 2 * Math.PI : angle;

    if (!isNaN(angle) &&
        ipDirMag > 0.05) {
        angleLog.add(angle);
        var filteredAngle = angleLog.value();

        sphero.driftAngle = filteredAngle;
    }

    return {
        ipData: {
            name: name,
            pos: pos,
            drift: sphero.driftAngle ? sphero.driftAngle : 0.0,
            angle: ipDirAngle,
        },
        kalmanData: {
            name: name,
            d: sphero.kalmanModel.x_k.elements
        },
    }
}

// var arenaSize = {
//     x: 1.0,
//     y: 1.0,
// };
var imageSize = {
    x: 1280.0,
    y: 720.0,
};

function pixelToPosition(pos) {
    return {
        x: pos.x / (imageSize.x * 0.5) - 1.0,
        y: (imageSize.y - pos.y) / (imageSize.y * 0.5) - 1.0,
    }
}
