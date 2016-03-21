var ipc = require('node-ipc');

ipc.config.id = 'hello';
// ipc.config.retry = 1500;

var id = 'world';

ipc.connectTo(id, _ => {
    ipc.of[id].on('connect', _ => {
        ipc.log('## connected to world ##'.rainbow);
        ipc.of[id].emit('message', 'hello')
    });
    ipc.of[id].on('disconnect', () => {
        ipc.log('disconnected from'.notice);
    });
    ipc.of[id].on('message', (data) => {
        ipc.log('got a message from world : '.debug, data);
    });
});
