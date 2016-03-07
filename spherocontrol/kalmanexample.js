var sylvester = require('sylvester');
var Matrix = sylvester.Matrix;

KalmanModel = (function() {

    function KalmanModel(x_0, P_0, F_k, Q_k) {
        this.x_k = x_0;
        this.P_k = P_0;
        // F_k is usuall A, the model for predicting the next value
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

KalmanObservation = (function() {

    function KalmanObservation(z_k, H_k, R_k) {
        this.z_k = z_k; //observation
        this.H_k = H_k; //observation model
        this.R_k = R_k; //observation noise covariance
    }

    return KalmanObservation;
})();

// initial state
var x_0 = $V([1, 2]);

// initial prediciton error (always 1)
var P_0 = $M([
    [1, 0],
    [0, 1]
]);

// the input model
var F_k = $M([
    [1, 0],
    [0, 1]
]);

// not sure - something about nonlinearity
var Q_k = $M([
    [1, 0],
    [0, 1]
]);
var KM = new KalmanModel(x_0, P_0, F_k, Q_k);


// observation value
var z_k = $V([1, 2]);

// observation model
var H_k = $M([
    [1, 0],
    [0, 1]
]);

// noise value
var R_k = $M([
    [1, 0],
    [0, 1],
]);

var KO = new KalmanObservation(z_k, H_k, R_k);

for (var i = 0; i < 200; i++) {
    z_k = $V([0.5 + Math.random(), 1.5 + Math.random()]);
    KO.z_k = z_k;
    KM.update(KO);
    console.log(JSON.stringify(KM.x_k.elements));
}
