'use strict';
var sylvester = require('sylvester');
var Matrix = sylvester.Matrix;
var KalmanModel = (function() {

    function KalmanModel(x_0, P_0, F_k, Q_k) {
        this.x_k = x_0;
        this.P_k = P_0;
        this.F_k = F_k;
        this.Q_k = Q_k;
    }

    KalmanModel.prototype.update = function(o) {
        this.I = Matrix.I(this.P_k.rows());
        //init
        this.x_k_ = this.x_k;
        this.P_k_ = this.P_k;

        //Predict
        this.x_k_k_ = this.F_k.x(this.x_k_);
        this.P_k_k_ = this.F_k.x(this.P_k_.x(this.F_k.transpose())).add(this.Q_k);

        //update
        this.y_k = o.z_k.subtract(o.H_k.x(this.x_k_k_)); //observation residual
        this.S_k = o.H_k.x(this.P_k_k_.x(o.H_k.transpose())).add(o.R_k); //residual covariance
        this.K_k = this.P_k_k_.x(o.H_k.transpose().x(this.S_k.inverse())); //Optimal Kalman gain
        this.x_k = this.x_k_k_.add(this.K_k.x(this.y_k));
        this.P_k = this.I.subtract(this.K_k.x(o.H_k)).x(this.P_k_k_);
    }

    return KalmanModel;
})();

var KalmanObservation = (function() {

    function KalmanObservation(z_k, H_k, R_k) {
        this.z_k = z_k; //observation
        this.H_k = H_k; //observation model
        this.R_k = R_k; //observation noise covariance
    }

    return KalmanObservation;
})();


function vec2log(size, log) {
    log = log || [];
    // var log = [];

    function add(item) {
        log.push([item, new Date().getTime()]);
        if (log.length > size) {
            log.splice(0, 1);
        }
    }

    function lastEntry(num) {
        if (!num)
            num = 0;
        return log[log.length - 1 - num];
    }

    function closestEntry(time) {
        var closest = log[0];
        var distance = Math.abs(log[0][1] - time);
        var j = 0;
        for (let i = 1; i < log.length; i++) {
            var newDist = Math.abs(log[i][1] - time);
            if (newDist < distance) {
                closest = log[i];
                distance = newDist;
                j = i;
            }
        }
        return {
            closest,
            index: j
        };
    }

    function average() {
        var sum = log.reduce(
            (prev, cur) => {
                return {
                    x: cur[0].x + prev.x,
                    y: cur[0].y + prev.y
                };
            }, {
                x: 0,
                y: 0
            }
        );

        return {
            x: sum.x / log.length,
            y: sum.y / log.length
        }
    }
    // api.add = add;
    // api.average = average;
    // api.lastEntry = lastEntry;
    // api.closestEntry = closestEntry;
    return {
        add,
        average,
        lastEntry,
        closestEntry,
        log
    };
    // return api;
}

// function Filter(size) {
//     var log = [];

//     return {
//         add: function(item) {
//             log.push(item);
//             if (log.length > size) {
//                 log.splice(0, 1);
//             }
//         },
//         value: function() {
//             return log.reduce((e, i) => e + i, 0) / log.length;
//         }
//     }
// }

// function XYFilter(size) {
//     var log = [];

//     return {
//         add: function(item) {
//             log.push(item);
//             if (log.length > size) {
//                 log.splice(0, 1);
//             }
//         },
//         value: function() {
//             var sum = log.reduce((e, i) => {
//                 return {
//                     x: e.x + i.x,
//                     y: e.y + i.y
//                 }
//             }, {
//                 x: 0,
//                 y: 0
//             });

//             return {
//                 x: sum.x / log.length,
//                 y: sum.y / log.length
//             }
//         }
//     }
// }

module.exports = {
    KalmanModel,
    KalmanObservation,
    vec2log,
    // Filter
}