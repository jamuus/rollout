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
        [0.001, 0, 0, 0],
        [0, 0.001, 0, 0],
        [0, 0, 0.001, 0],
        [0, 0, 0, 0.001],
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
        [10, 0, 0, 0],

        [0, 10, 0, 0],
        [0, 0, 0.1, 0],
        [0, 0, 0, 0.1],
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
        kalmanObservation: KO,
        angleLog: Filter(500)
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

var transformCorners;
var cv = require("opencv");

var imageSize = {
    x: 1280.0,
    y: 720.0,
};

function objxytoarray(obj) {
    return [obj.x, obj.y];
}

function objlistxytoarray(list) {
    return list.map(e => objxytoarray(e));
}

function warpPerspectiveList(p, m) {
    var q = [];
    for (var i = 0; i < p.length; i += 2) {
        q.push(warpPerspective(p, m, i));
    }
    return q;
}

function warpPerspective(p, m, i) {
    if (typeof(i) === "undefined") {
        i = 0;
    }

    var q = [
        p[i + 0] * m.get(0, 0) + p[i + 1] * m.get(0, 1) + m.get(0, 2),
        p[i + 0] * m.get(1, 0) + p[i + 1] * m.get(1, 1) + m.get(1, 2)
    ];

    var f = p[i + 0] * m.get(2, 0) + p[i + 1] * m.get(2, 1) + m.get(2, 2);

    q[i + 0] /= f;
    q[i + 1] /= f;

    return q;
}

var spheroScale = 22;
var outputScale = 20;

var debugLog = console.log;

module.exports = function(spheroManager, fn) {
    var dataOut = fn.dataOut;

    // called by web client to test forces
    fn.forceCallback(function(data) {
        spheros.ybr.force(data.direction, data.force);
        spheros.boo.force(data.direction, data.force);
    });

    fn.transformCallback(function(data) {
        transformCorners = data.corners;
        if (transformCorners) {
            var corners = objlistxytoarray(transformCorners);
            var target = [0, 0, imageSize.x, 0, imageSize.x, imageSize.y, 0, imageSize.y];

            var transform = new cv.Matrix.Zeros(3, 3).getPerspectiveTransform(corners, target);

            console.log(transform.row(0));
            console.log(transform.row(1));
            console.log(transform.row(2));
            console.log();
        }
    });

    fn.spheroScaleCallback(function(data) {
        if (data.spheroScale)
            spheroScale = data.spheroScale;
        if (data.outputScale)
            outputScale = data.outputScale;
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

    spheroState.kalmanObservation.z_k = $V(
        [0, 0,
            absVelX / (1000 * spheroScale),
            absVelY / (1000 * spheroScale)
        ]
    );
    spheroState.kalmanObservation.H_k =
        $M([
            [0, 0, 0, 0],
            [0, 0, 0, 0],
            [0, 0, 1, 0],
            [0, 0, 0, 1],
        ]);
    spheroState.kalmanObservation.F_k = $M([
        [1, 0, diff, 0],
        [0, 1, 0, diff],
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
    // console.log(spheroState.kalmanModel.P_k);
    // console.log();

    // var expFilterVal = 3;
    // spheroState.pos.x = spheroState.pos.x - spheroState.pos.x / expFilterVal + (-spheroState.kalmanModel.x_k.elements[0] * outputScale) / expFilterVal
    // spheroState.pos.y = spheroState.pos.y - spheroState.pos.y / expFilterVal + (-spheroState.kalmanModel.x_k.elements[1] * outputScale) / expFilterVal

    var pos = {
        x: -spheroState.kalmanModel.x_k.elements[0] * outputScale,
        y: -spheroState.kalmanModel.x_k.elements[1] * outputScale + 1
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
                // d: [-spheroState.pos.x / outputScale, -spheroState.pos.y / outputScale]
        }
    }
}

var ipScale = 40;

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
    // sphero.pos = pos;

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
        spheroDirMag > 0.05) {
        sphero.angleLog.add(angle);
        var filteredAngle = sphero.angleLog.value();

        sphero.driftAngle = filteredAngle;
    }

    return {
        ipData: {
            name: name,
            pos: pos,
            drift: sphero.driftAngle ? sphero.driftAngle : 0.0,
            angle: ipDirAngle,
            pos3d: transformedPosition
        },
        // kalmanData: {
        //     name: name,
        //     d: sphero.kalmanModel.x_k.elements
        // },
    }
}

var angleOffset = 0; // camera angle offset
var h = 2; // 2 meters high
var verticalFov = 60 / 360 * Math.PI * 2; // 60 degrees in radiuns
var radiunsPerYPixel = verticalFov / imageSize.y;
var focalLength = 0.01; // 10 mm?

var resetAngle = angleOffset + verticalFov / 2;

var rotatu = $M([
    [1, 0, 0],
    [0, Math.cos(resetAngle), -Math.sin(resetAngle)],
    [0, Math.sin(resetAngle), Math.cos(resetAngle)]
]);

function pixelToPosition(pos) {
    var y = imageSize.y - pos.y;

    var Z = h / Math.cos(angleOffset + radiunsPerYPixel * y);
    var X = focalLength * pos.x / Z;
    var Y = focalLength * y / Z;
    var res = rotatu.x($V([X, Y, Z]));
    return {
        x: pos.x / (imageSize.x * 0.5) - 1.0,
        y: (imageSize.y - pos.y) / (imageSize.y * 0.5) - 1.0,
    }
    // return {
    //     x: res.elements[0],
    //     y: res.elements[1],
    //     z: res.elements[2]
    // }

}
