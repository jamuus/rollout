"use strict";

var Net = require("dgram");
var log = console.log;

var IP = "172.23.88.196";
var PORT = 7779;

var MESSAGE_TYPE_TEST               = 0x00;
var MESSAGE_TYPE_REMOVE_SPHERO      = 0x01;
var MESSAGE_TYPE_SET_ENDIANNESS     = 0x02;
var MESSAGE_TYPE_UPDATE_STATE       = 0x04;
var MESSAGE_TYPE_ROLL_SPHERO        = 0x08;
var MESSAGE_TYPE_SERVER_DISCOVER    = 0x10;
var MESSAGE_TYPE_SPHERO_SHOOT       = 0x20;
var MESSAGE_TYPE_SPHERO_POWERUP     = 0x40;
var MESSAGE_TYPE_PAUSE_GAME         = 0x80;
var MESSAGE_TYPE_NODE_INIT          = 0x11;
var MESSAGE_TYPE_APP_INIT           = 0x21;

var socket = Net.createSocket("udp4");
var isLittleEndian = true;

socket.on("listening", function() {
    //console.log("Listening on " + IP + ":" + PORT + "...");
    console.log("Listening on port " + PORT + "...");
});

var client;

socket.on("message", function(data, remote) {
    //console.log("Received message from " + remote.address + ":" + remote.port + ".");
    if (!client)
        setInterval(sendState, 1000 / 60);
    client = remote;
    var type = data[0];
    switch (type) {
        case MESSAGE_TYPE_TEST:
            console.log("Received test message.");
            break;
        case MESSAGE_TYPE_REMOVE_SPHERO:
            var device = data.toString("ascii", 2);
            console.log("Request to remove sphero '" + device + "'.");
            break;
        case MESSAGE_TYPE_SET_ENDIANNESS:
            isLittleEndian = data[1];
            if (isLittleEndian)
                console.log("Set server to little endian.");
            else
                console.log("Set server to big endian.");
            break;
        case MESSAGE_TYPE_ROLL_SPHERO:
            var direction = isLittleEndian ? data.readFloatLE(1) : data.readFloatBE(1);
            var force = isLittleEndian ? data.readFloatLE(5) : data.readFloatBE(5);
            // todo fix
            var name = data.slice(1).toString("ascii", 9);

            console.log("Rolling sphero '" + name + "' " + direction + " with force " + force + ".");
            //state[name].force(direction, force);
            break;
        case MESSAGE_TYPE_SERVER_DISCOVER:
            //console.log("Received discovery request method from " + remote.address + ":" + remote.port + ", sent response.");
            //var msg = new Buffer(1);
            //msg[0] = MESSAGE_TYPE_SERVER_DISCOVER;
            //socket.send(msg, 0, msg.length, remote.port, remote.address, function(err) {
            //    if (err)
            //        throw err;
            //});
            break;
        case MESSAGE_TYPE_SPHERO_SHOOT:
            var weapon = data[1];
            var direction = isLittleEndian ? data.readFloatLE(2) : data.readFloatBE(2);
            var sphero = data.toString("ascii", 8);
            console.log("Sphero '" + sphero + "' shoots weapon ID " + weapon + " in direction " + direction + ".");
            break;
        case MESSAGE_TYPE_SPHERO_POWERUP:
            var powerup = data[1];
            var sphero = data.toString("ascii", 3);
            console.log("Sphero '" + sphero + "' uses powerup ID " + powerup + ".");
            break;
        case MESSAGE_TYPE_PAUSE_GAME:
            var sphero = data.toString("ascii", 2);
            console.log("Sphero '" + sphero + "' requested game pause.");
            break;
        default:
            console.log("Unknown message.");
            break;
    }
});

socket.bind(PORT); // I removed ", IP" and it works, no idea why

var identifier = new Buffer(1);
identifier[0] = MESSAGE_TYPE_NODE_INIT;
console.log("sending id");
socket.send(identifier, 0, identifier.length, 7777, IP, function(err) { if (err) throw err; });
console.log("sent id");

function spheroState() {
    var api = {};

    var manager = require('./spheroManager')();
    var spheroLoc = require('./spheroLoc.js')(manager);

    return spheroLoc;
}

var state = spheroState();

function sendState() {
    var message = new Buffer(1);
    message[0] = MESSAGE_TYPE_UPDATE_STATE;
    for (var name in state) {
        var sphero = state[name];
        var idx = 0;
        var length = new Buffer(1);
        length[0] = name.length;
        var header = Buffer.concat([
            length,
            new Buffer(name, 'ascii')
        ]);
        var buf = new Buffer(5 * 4);

        if (isLittleEndian) {
            buf.writeFloatLE(sphero.dx, idx);
            buf.writeFloatLE(sphero.dy, idx + 4);
        } else {
            buf.writeFloatBE(sphero.dx, idx);
            buf.writeFloatBE(sphero.dx, idx + 4);
        }
        idx += 8;
        if (isLittleEndian) {
            buf.writeFloatLE(sphero.x, idx);
            buf.writeFloatLE(sphero.y, idx + 4);
        } else {
            buf.writeFloatBE(sphero.x, idx);
            buf.writeFloatBE(sphero.x, idx + 4);
        }
        idx += 8;
        if (isLittleEndian) {
            buf.writeFloatLE(sphero.batteryVoltage, idx);
        } else {
            buf.writeFloatBE(sphero.batteryVoltage, idx);
        }
        message = Buffer.concat([message, header, buf]);
    }
    socket.send(message, 0, message.length, client.port + 1, client.address, function(err) {
        if (err)
            throw err;
        // console.log("Sent state to client " + client.address + ":" + (client.port + 1) + ".");
    })
}

function addString(buf, idx, string) {
    buf[idx] = string.length;
    buf.write(string, 1 + idx, string.length, 'ascii');
    return idx + string.length + 1;
}
