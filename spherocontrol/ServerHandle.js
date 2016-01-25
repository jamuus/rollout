function ServerHandle(name, ip) {
    this._name = name;
    this._ip   = ip;
}

ServerHandle.prototype.getName = function() {
    return this._name;
};

ServerHandle.prototype.getIP = function() {
    return this._ip;
};

ServerHandle.prototype.toString = function() {
    return "\"" + this._name + "\" (" + this._ip + ":7777)";
}

module.exports = ServerHandle;