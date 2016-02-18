var spheroIds = [
    'ybr',
    'boo',
];

var spheros = {
    "ybr": initSphero(),
    "boo": initSphero(),
}

function nothing() {
    console.log('nothing b0ss');
}

function initSphero() {
    return {
        x: 0,
        y: 0,
        dx: 0,
        dy: 0,
        lastVelocityUpdate: -1,
        batteryVoltage: -1,
        force: nothing,
        driftAngle: 0
    }
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
            return log.reduce((e, i) => e + i, 0);
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
            return log.reduce((e, i) => {
                return {
                    x: e.x + i.x,
                    y: e.y + i.y
                }
            }, {
                x: 0,
                y: 0
            });
        }
    }
}

var debugLog = console.log;

module.exports = function(spheroManager, fn) {
    var dataOut = fn.dataOut;

    fn.forceCallback(function(data) {
        spheros.boo.force(data.direction, data.force);
    });

    spheroManager.onSpheroConnect(function(newSphero) {
        debugLog('Sphero', newSphero.name, 'connected.');

        var spheroData = spheros[newSphero.name.toLowerCase().indexOf("ybr") !== -1 ? "ybr" : "boo"];
        spheroData.force = newSphero.force;

        newSphero.newDataCallback(function(data, type) {
            for (var dataName in data) {
                spheroData[dataName] = data[dataName];
            }
            if (type === 'velocity') {
                if (spheroData.lastVelocityUpdate !== -1) {
                    var now = new Date().getTime();
                    var diff = now - spheroData.lastVelocityUpdate;

                    newSpheroData(newSphero, spheroData, diff);

                    spheroData.lastVelocityUpdate = now;
                } else {
                    spheroData.lastVelocityUpdate = new Date().getTime();
                }
            }
        });
    });

    // var spheroFilter = {Filter(10), Filter(10)]
    var spheroFilter = {};

    function newSpheroData(sphero, data, dt) {
        // data.dx, data.dy
        if (!spheroFilter[sphero.name])
            spheroFilter[sphero.name] = XYFilter(10);
        spheroFilter[sphero.name].add({
            x: data.dx,
            y: data.dy
        });
        var filteredData = spheroFilter[sphero.name].value();
        dataOut({
            spheroData: {
                data: filteredData,
                name: sphero.name
            }
        });

        sphero.dx = filteredData.dx;
        sphero.dy = filteredData.dy;
    }

    ip(dataOut);


    return spheros;
}

function ip(dataOut) {
    var PORT = 1337;
    var HOST = '127.0.0.1';

    var dgram = require('dgram');
    var server = dgram.createSocket('udp4');

    server.on('listening', function() {
        var address = server.address();
        debugLog('Listening for image processing data on', address.address + ":" + address.port);
    });

    var bef = new Date().getTime();

    server.on('message', function(message, remote) {
        var data = {};
        var parts = message.toString().split(',');
        data.id = parts[0];
        // absolute position update
        data.x = parseInt(parts[1]);
        data.y = parseInt(parts[2]);

        var sphName = spheroIds[data.id];
        if (spheros[sphName]) {
            var aft = new Date().getTime();
            var diff = aft - bef;
            newIpData(sphName, spheros[sphName], data, diff);

            // debugLog(sphName, data.x, data.y);
            bef = aft;
        }
    });

    var angleLog = Filter(10);

    var posLog = [XYFilter(10), XYFilter(10)];
    var lastPos = [{
        x: 0,
        y: 0
    }, {
        x: 0,
        y: 0
    }];

    function newIpData(name, sphero, data, dt) {
        // data.x,  data.y, data.id
        var angle, dx, dy;

        if (data.x === -1 || data.y === -1) {
            // lost sphero
            return;
        }

        posLog[data.id].add(data);

        var filteredPos = posLog[data.id].value();

        dx = filteredPos.x - lastPos[data.id].x;
        dy = filteredPos.y - lastPos[data.id].y;

        lastPos[data.id] = filteredPos;

        var ipmag = Math.sqrt(dx * dx + dy * dy);
        var sphmag = Math.sqrt(sphero.dx * sphero.dx + sphero.dy * sphero.dy);

        angle = (dx * sphero.dx + dy * sphero.dy) / (ipmag * sphmag);
        angle = Math.acos(angle);

        // console.log(angle);
        if (!isNaN(angle))
            angleLog.add(angle);

        var filteredAngle = angleLog.value();

        dataOut({
            ipData: {
                name: name,
                pos: data,
                drift: filteredAngle,
                angle: Math.atan2(dy, dx)
            }
        });
        sphero.driftAngle = filteredAngle;
    }

    server.bind(PORT);
}

/*
    get a dx from ip
        create movement vector from last data and this
        if sphero has travelled along a line, use this vector
        low pass of differences
    get a dx from sphero data
        straight up diff vector
        low pass of diff vectors

    dot product
        update known orientation - low pass/exp decay
*/

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
