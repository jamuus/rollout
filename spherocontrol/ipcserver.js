var ipc = require('node-ipc');

ipc.config.id = 'world';

ipc.serve(_ => {
    ipc.server.on('message', (data, socket) => {
        ipc.log('got a message : '.debug, data);
        ipc.server.emit(socket, 'message', data + ' world!');
    });
});

ipc.server.start();
