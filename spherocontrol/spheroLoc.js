var debug = console.log;

module.exports = (function() {
    var api = {
        newData: function(data) {}
    };

    var PORT = 1337;
    var HOST = '127.0.0.1';

    var dgram = require('dgram');
    var server = dgram.createSocket('udp4');

    server.on('listening', function() {
        var address = server.address();
        debug('UDP Server listening on ' + address.address + ":" + address.port);
    });

    server.on('message', function(message, remote) {
        var data = {};
        var parts = message.toString().split(',');
        data.id = parts[0];
        data.x = parts[1];
        data.y = parts[2];
        debug(data);
        api.newData(data);
    });

    server.bind(PORT, HOST);

    return api;
})();
