"use strict";

var Net = require("dgram");
var ServerHandle = require("./ServerHandle.js");

var PORT        = 7779;
var UNITY_PORT  = 7777;

var MessageType = {
    TEST                : 0x00,
    REMOVE_SPHERO       : 0x01,
    SET_ENDIANNESS      : 0x02,
    UPDATE_STATE        : 0x04,
    ROLL_SPHERO         : 0x08,
    SERVER_DISCOVER     : 0x10,
    SPHERO_SHOOT        : 0x20,
    SPHERO_POWER_UP     : 0x40,
    PAUSE_GAME          : 0x80,
    NODE_INIT           : 0x11,
    APP_INIT            : 0x21
};

var udpOutgoing = Net.createSocket("udp4");
var udpIncoming = Net.createSocket("udp4");
var isLittleEndian = true;
var discoveredServers = [];
var connectedServer = undefined;
var state = undefined;

function discover() {
    var discoverSocket = Net.createSocket("udp4");
    var incomingSocket = Net.createSocket("udp4");

    incomingSocket.on("listening", function() {
        console.log("Searching for servers...");
    });

    incomingSocket.on("message", function(data, remote) {
        if (data[0] == MessageType.SERVER_DISCOVER) {
            var name = data.toString("ascii", 2);
            var handle = new ServerHandle(name, remote.address);
            discoveredServers.push(handle);
            console.log((discoveredServers.length - 1) + ": " + handle.toString());
        }
    });

    incomingSocket.bind(PORT + 1);
    discoverSocket.bind(PORT, function() {
        discoverSocket.setBroadcast(true);
        var buf = new Buffer(1);
        buf[0] = MessageType.SERVER_DISCOVER;
        discoverSocket.send(buf, 0, buf.length, UNITY_PORT, "255.255.255.255", function() {
            discoverSocket.close();
        });
    });

    setTimeout(function() {
        incomingSocket.close();
        if (discoveredServers.length > 0) {
            console.log("\nSelect server to join: ");

            process.stdin.resume();
            process.stdin.setEncoding("utf8");
            process.stdin.on("data", function(data) {
                var idx = parseInt(data, 10);
                if (idx >= discoveredServers.length) {
                    console.log("Invalid server. Options are: ");
                    for (var i = 0; i < discoveredServers.length; ++i)
                        console.log(i + ": " + discoveredServers[i].toString());
                    console.log("Select server to join: ");
                } else {
                    process.stdin.pause();
                    var server = discoveredServers[parseInt(data, 10)];
                    connect(server);
                }
            });
        } else {
            console.log("None found.");
            process.exit();
        }
    }, 2000);
}

function connect(server) {
    console.log("\nConnecting to " + server.toString());

    udpIncoming.on("listening", function() {
        console.log("Listening on port " + (PORT + 1));
    });

    udpIncoming.on("message", function(data, remote) {
        switch(data[0]) {
            case MessageType.SET_ENDIANNESS:
                isLittleEndian = data[1];
                console.log("Set endianness to " + data[1]);
                break;
            default:
                console.log("Unknown message received.");
                break;
        }
    });

    udpIncoming.bind(PORT + 1);
    udpOutgoing.bind(PORT, function() {
        var buf = new Buffer(1);
        buf[0] = MessageType.NODE_INIT;
        udpOutgoing.send(buf, 0, buf.length, UNITY_PORT, server.ip);
        connectedServer = server;
        state = spheroState();
        setInterval(sendState, 1000 / 60);
    });
}

function spheroState() {
    var manager = require("./spheroManager")();

    var api = { };
    var instances = [];

    manager.onSpheroConnect(function(newSphero) {
        instances.push(newSphero);
        var spheroData = api[newSphero.name] = {
            x: 0,
            y: 0,
            dx: 0,
            dy : 0,
            lastVelocityUpdate: -1,
            batteryVoltage: 0,
            force: newSphero.force
        };

        newSphero.newDataCallback(function(data, type) {
            for (var dataName in data) {
                api[newSphero.name][dataName] = data[dataName];
            }
            if (type === "velocity") {
                if (spheroData.lastVelocityUpdate !== -1) {
                    var now = new Date().getTime();
                    var diff = now - spheroData.lastVelocityUpdate;

                    spheroData.x += (diff / 1000) * spheroData.dx;
                    spheroData.y += (diff / 1000) * spheroData.dy;

                    spheroData.lastVelocityUpdate = now;
                }
            }
        });
    });

    return api;
}

function sendState() {

    var message = new Buffer(1);
    message[0] = MessageType.UPDATE_STATE;

    for (var name in state) {
        var sphero = state[name];
        var buf = new Buffer(1 + name.length + 5 * 4);
        buf[0] = name.length;
        buf.write(name, 1, name.length, "ascii");

        var idx = 1 + name.length;
        if (isLittleEndian) {
            buf.writeFloatLE(sphero.dx, idx);
            idx += 4;
            buf.writeFloatLE(sphero.dy, idx);
            idx += 4;
            buf.writeFloatLE(sphero.x, idx);
            idx += 4;
            buf.writeFloatLE(sphero.y, idx);
            idx += 4;
            buf.writeFloatLE(sphero.batteryVoltage, idx);
        } else {
            // TODO
        }

        message = Buffer.concat([message, buf]);
    }

    udpOutgoing.send(message, 0, message.length, UNITY_PORT, connectedServer.ip, function(err) {
        if (err) throw err;
    });
}

discover();