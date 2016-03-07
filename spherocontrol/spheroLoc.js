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

function initSphero() {
    return {
        ipPos: vec2log(10),
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
        dy: 0
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
            return log.reduce((e, i) => e + i, 0) / size;
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
                x: sum.x / size,
                y: sum.y / size
            }
        }
    }
}

var debugLog = console.log;

module.exports = function(spheroManager, fn) {
    var dataOut = fn.dataOut;

    // called by web client to test forces
    fn.forceCallback(function(data) {
        spheros.ybr.force(data.direction, data.force);
        spheros.boo.force(data.direction, data.force);
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
                newSpheroData(data, spheroState);
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
            dataOut(newIpData(sphName, spheroState, data));
        }
    });


    server.bind(PORT);
}

var angleLog = Filter(30);
var lastPos = [{
    x: 0,
    y: 0
}, {
    x: 0,
    y: 0
}];

var bef = new Date().getTime();


function newSpheroData(data, spheroState) {
    spheroState.spheroVel.add({
        x: data.dx / 1000.0,
        y: data.dy / 1000.0
    });

    var absVelX = data.dx * Math.cos(spheroState.driftAngle) -
        data.dy * Math.sin(spheroState.driftAngle);
    var absVelY = data.dx * Math.sin(spheroState.driftAngle) +
        data.dy * Math.cos(spheroState.driftAngle);

    spheroState.absSpheroVel = {
        x: absVelX,
        y: absVelY,
    };

    var aft = new Date().getTime();
    // number of seconds past
    var diff = (aft - bef) / 1000.0;
    bef = aft;

    // update pos in meters
    spheroState.pos.x += (absVelX * diff);
    spheroState.pos.y += (absVelY * diff);
    // console.log(spheroState.pos);
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
        y: -data.y
    }

    var transformedPosition = pixelToPosition(ipData);

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

    ipDirAngle = Math.atan2(ipDirVec.y, ipDirVec.x);
    spheroDirAngle = Math.atan2(spheroDirVec.y, spheroDirVec.x);
    angle = spheroDirAngle - ipDirAngle;
    angle = angle > Math.PI ? angle - 2 * Math.PI : angle;
    angle = angle < -Math.PI ? angle + 2 * Math.PI : angle;

    if (!isNaN(angle) &&
        spheroDirMag > 0.1) {
        angleLog.add(angle);
    }

    var filteredAngle = angleLog.value();
    // console.log(spheroDirMag, ipDirMag);

    sphero.driftAngle = filteredAngle;

    return {
        spheroData: {
            name: name,
            data: sphero.absSpheroVel,
        },
        ipData: {
            name: name,
            pos: filteredPos,
            drift: filteredAngle ? filteredAngle : 0.0,
            angle: Math.atan2(ipDy, ipDx),
        }
    }
}

var arenaSize = {
    x: 2.0,
    y: 2.0,
};
var imageSize = {
    x: 1280,
    y: 720,
};

function pixelToPosition(pos) {
    return {
        x: pos.x / imageSize.x * arenaSize.x,
        y: pos.y / imageSize.y * arenaSize.y,
    }
}


KalmanModel = (function() {

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

/*


var kalmanLog = function() {}; // console.log;

var Matrix = require('sylvester');
var ones = Matrix.Matrix.Ones;
var I = Matrix.Matrix.I(4);
console.log(I.transpose());

function kalmanFilter() {
    var api = {
        X0: $M([0, 0, 0, 0]),
        R: I,
        P0: I,
    };

    api.A = $M([
        [1, 0, 1, 0],
        [0, 1, 0, 1],
        [0, 0, 1, 0],
        [0, 0, 0, 1],
    ]);

    api.updateX1 = function() {
        api.X1 = api.A.x(api.X0);
        kalmanLog('X1 =\n', api.X1);
    }

    api.updateP1 = function() {
        var A = api.A;
        var t1 = api.P0.x(api.A.transpose());
        var t2 = api.A.x(t1);

        api.P1 = t2;
        kalmanLog('P1 =\n', api.P1);
    }

    api.updateK = function() {
        var t1 = api.P1.x(I.transpose());
        var t2 = I.x(t1);
        var t3 = I.transpose().x(t2);
        var t4 = api.P1.x(t3);
        api.K = t4;
        kalmanLog('K =\n', api.K);
    }

    api.updateX0 = function(sensor) {
        kalmanLog('Sens=', sensor);
        var t1 = api.X1;
        var t2 = I.x(t1);
        var t3 = sensor.subtract(t2);
        var t4 = api.K.x(t3);
        var t5 = t1.add(t4);
        api.X0 = t5;
        kalmanLog('X0 =\n', api.X0);
    }

    api.updateP0 = function() {
        var t1 = api.K.x(I);
        var t2 = I.subtract(t1);
        var t3 = t2.x(api.P1);
        api.P0 = t3;
        kalmanLog('P0 =\n', api.P0);
    }

    return api;
}

var actualState = {
    x: 0,
    y: 0,
    dx: 0,
    dy: 0,
};

function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function noise() {
    return Math.random() - 0.5;
}

var filter = kalmanFilter();

var bef = new Date().getTime();

setInterval(function() {
    var aft = new Date().getTime();
    var diff = (aft - bef) / (5 * 1000);

    var dx = actualState.dx = Math.sin(diff);
    var dy = actualState.dy = Math.cos(diff);

    actualState.x += dx;
    actualState.y += dy;

    kalmanLog('Gen dx,dy', actualState.dx.toFixed(2), actualState.dy.toFixed(2));
    kalmanLog('Gen x,y  ', actualState.x.toFixed(2), actualState.y.toFixed(2));

    var sensorX = actualState.x + noise();
    var sensorY = actualState.y + noise();

    var sensorDx = dx + noise() / 2;
    var sensorDy = dy + noise() / 2;

    kalmanLog('Sens dx, dy', sensorDx.toFixed(2), sensorDy.toFixed(2));
    kalmanLog('Sens x, y  ', sensorX.toFixed(2), sensorY.toFixed(2));

    filter.updateX1();
    filter.updateP1();
    filter.updateK();
    filter.updateX0($M([sensorX, sensorY, sensorDx, sensorDy]));
    filter.updateP0();

    kalmanLog();
}, 2000);

*/
