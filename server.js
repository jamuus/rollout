"use strict";

var Net = require("dgram");

var IP      = "127.0.0.1";
var PORT    = 7777;

var MESSAGE_TYPE_TEST           = 0x00;
var MESSAGE_TYPE_REMOVE_SPHERO  = 0x01;
var MESSAGE_TYPE_SET_ENDIANNESS = 0x02;
var MESSAGE_TYPE_UPDATE_STATE   = 0x04;
var MESSAGE_TYPE_ROLL_SPHERO    = 0x08;

var socket = Net.createSocket("udp4");
var isLittleEndian = true;

socket.on("listening", function() {
    console.log("Listening on " + IP + ":" + PORT + "...");
});

var client;

socket.on("message", function(data, remote) {
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
            var direction = "";
            if (data[1] & 0x01)
                direction += "N";
            if (data[1] & 0x02)
                direction += "E";
            if (data[1] & 0x04)
                direction += "S";
            if (data[1] & 0x08)
                direction += "W";
            var force = isLittleEndian ? data.readFloatLE(2) : data.readFloatBE(2);
            var name = data.toString("ascii", 7);
            console.log("Rolling sphero '" + name + "' " + direction + " with force " + force + ".");
            sendState();
            break;
        default:
            console.log("Unknown message.");
            break;
    }
});

socket.bind(PORT, IP);

var instances = [{dname:"SPHERO-BOO",fname:"Boo",vel:{x:1.5,y:0.25}}];

function sendState() {
    var message = new Buffer(1);
    message[0] = MESSAGE_TYPE_UPDATE_STATE;
    for (var i = 0; i < instances.length; ++i) {
        var sphero = instances[i];
        var buf = new Buffer(sphero.dname.length + sphero.fname.length + 2 + 8);
        buf[0] = sphero.dname.length;
        buf.write(sphero.dname, 1, sphero.dname.length, "ascii");
        buf[1+sphero.dname.length] = sphero.fname.length;
        buf.write(sphero.fname, sphero.dname.length+2,sphero.fname.length,"ascii");
        var idx = sphero.dname.length+sphero.fname.length+2;
        if (isLittleEndian) {
            buf.writeFloatLE(sphero.vel.x, idx);
            buf.writeFloatLE(sphero.vel.y, idx+4);
        } else {
            buf.writeFloatBE(sphero.vel.x, idx);
            buf.writeFloatBE(sphero.vel.y, idx+4);
        }
        message = Buffer.concat([message, buf]);
    }
    
    socket.send(message, 0, message.length, client.port+1, client.address, function(err) {
        if (err)
            throw err;
        console.log("Sent state to client " + client.address + ":" + (client.port+1) + ".");
    })
}
