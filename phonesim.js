/*
 * PHONE CONNECTION PROCESS
 *  + Send server discovery broadcast to locate server.
 *  + Send app init to server, receive endianness and sphero name.
 *  + Done.
 *
 */

"use strict";

var MESSAGE_TYPE_SERVER_DISCOVER    = 0x10;
var MESSAGE_TYPE_APP_INIT           = 0x21;
var MESSAGE_TYPE_UPDATE_STATE       = 0x04;

var SERVER_PORT = 7777;

var Datagram = require("dgram");

var socket = Datagram.createSocket("udp4");
var isLittleEndian = true;
var server = { };
var sphero = { };
var spheroName = "";

var bufDiscover = new Buffer(1);
bufDiscover[0] = MESSAGE_TYPE_SERVER_DISCOVER;
var bufInit = new Buffer(1);
bufInit[0] = MESSAGE_TYPE_APP_INIT;

socket.on("listening", function() {
    console.log("Started listening.");
});

socket.on("message", function(data, remote) {
    switch (data[0]) {
        case MESSAGE_TYPE_SERVER_DISCOVER:
            server.name = data.toString("ascii", 2);
            server.ip = remote.address;
            console.log("found server: ", server);
            socket.send(bufInit, 0, bufInit.length, SERVER_PORT, server.ip);
            break;
        case MESSAGE_TYPE_APP_INIT:
            isLittleEndian = data[1];
            spheroName = data.toString("ascii", 3);
            console.log("app init - endian: " + isLittleEndian + ", sphero: " + spheroName + ".");
            break;
        case MESSAGE_TYPE_UPDATE_STATE:
            var index = 1;
            sphero.name = data.toString("ascii", index + 1, index + 1 + data[index]);
            index += sphero.name.length + 1;
            sphero.health = data.readFloatLE(index);
            index += 4;
            sphero.sheild = data.readFloatLE(index);
            index += 4;
            sphero.voltage = data.readFloatLE(index);
            index += 4;
            sphero.weapons = [];
            var weaponcount = data[index];
            ++index;
            for (var i = 0; i < weaponcount; ++i)
                sphero.weapons.push(data[index + i]);
            index += weaponcount;
            sphero.powerups = [];
            var powerupcount = data[index];
            ++index;
            for (var i = 0; i < powerupcount; ++i)
                sphero.powerups.push(data[index + i]);
            console.log("sphero state: ", sphero);
            break;
        default:
            console.log("unknown message: " + data[0] + ".");
            break;
    }
});

socket.send(bufDiscover, 0, bufDiscover.length, SERVER_PORT, "localhost");
