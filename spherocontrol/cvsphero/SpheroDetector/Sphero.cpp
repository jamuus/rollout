#include "Sphero.h"



Sphero::Sphero()
{
    //set values for default constructor
    setType("null");
    setColour(Scalar(0,0,0));
}

Sphero::Sphero(string name){

    if(name=="boo"){

        //TODO: use "calibration mode" to find HSV min
        //and HSV max values
        setType(name);
        setHSVmin(Scalar(7,175,154));
        setHSVmax(Scalar(15,256,256));

        //BGR value for Orange:
        setColour(Scalar(0,128,256));
    }
    if(name=="ybr"){

        //TODO: use "calibration mode" to find HSV min
        //and HSV max values
        setType(name);
        setHSVmin(Scalar(98,239,126));
        setHSVmax(Scalar(112,255,256));

        //BGR value for Yellow:
        setColour(Scalar(256,0,0));
    }
}

Sphero::~Sphero(void)
{
}

int Sphero::getXPos(){

    return Sphero::xPos;

}

void Sphero::setXPos(int x){

    Sphero::xPos = x;

}

int Sphero::getYPos(){

    return Sphero::yPos;

}

void Sphero::setYPos(int y){

    Sphero::yPos = y;

}

Scalar Sphero::getHSVmin(){

    return Sphero::HSVmin;

}
Scalar Sphero::getHSVmax(){

    return Sphero::HSVmax;
}

void Sphero::setHSVmin(Scalar min){

    Sphero::HSVmin = min;
}


void Sphero::setHSVmax(Scalar max){

    Sphero::HSVmax = max;
}