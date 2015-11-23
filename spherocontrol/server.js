"use strict";

var Net = require("dgram");
var log = console.log;

var IP = "127.0.0.1";
var PORT = 7777;

var MESSAGE_TYPE_TEST = 0x00;
var MESSAGE_TYPE_REMOVE_SPHERO = 0x01;
var MESSAGE_TYPE_SET_ENDIANNESS = 0x02;
var MESSAGE_TYPE_UPDATE_STATE = 0x04;
var MESSAGE_TYPE_ROLL_SPHERO = 0x08;

var socket = Net.createSocket("udp4");
var isLittleEndian = true;

socket.on("listening", function() {
    console.log("Listening on " + IP + ":" + PORT + "...");
});

var client;

socket.on("message", function(data, remote) {
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

            // console.log("Rolling sphero '" + name + "' " + direction + " with force " + force + ".");
            state[name].force(direction, force);
            break;
        default:
            console.log("Unknown message.");
            break;
    }
});

socket.bind(PORT, IP);


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
