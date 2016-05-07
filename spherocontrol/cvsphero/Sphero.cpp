#include "Sphero.h"



Sphero::Sphero()
{
    //set values for default constructor
    setType("null");
    setColour(Scalar(0, 0, 0));
}

Sphero::Sphero(string name)
{

    if (name == "boo") {

        //TODO: use "calibration mode" to find HSV min
        //and HSV max values
        setType(name);

        // orange cover
        // setHSVmin(Scalar(5, 175, 154));
        // setHSVmax(Scalar(20, 256, 256));

        // red tape
        // setHSVmin(Scalar(122, 93, 98));
        // setHSVmax(Scalar(189, 256, 230));
        // H_MIN 122
        // H_MAX 189
        // S_MIN 93
        // S_MAX 256
        // V_MIN 98
        // V_MAX 230


        // red light
        setHSVmin(Scalar(0, 75, 196));
        setHSVmax(Scalar(51, 256, 256));
        // H_MIN 0
        // H_MAX 51
        // S_MIN 75
        // S_MAX 256
        // V_MIN 196
        // V_MAX 256


        // yellow light
        // setHSVmin(Scalar(0, 0, 205));
        // setHSVmax(Scalar(51, 67, 256));
        // H_MIN 0
        // H_MAX 51
        // S_MIN 0
        // S_MAX 67
        // V_MIN 205
        // V_MAX 256

        //BGR value for Orange:
        setColour(Scalar(0, 128, 256));
    }
    if (name == "ybr") {

        //TODO: use "calibration mode" to find HSV min
        //and HSV max values
        setType(name);


        // setHSVmin(Scalar(25, 71, 180));
        // setHSVmax(Scalar(120, 186, 256));


        // blue light 
        setHSVmin(Scalar(70, 173, 151));
        setHSVmax(Scalar(146, 256, 256));
        // H_MIN 70
        // H_MAX 146
        // S_MIN 173
        // S_MAX 256
        // V_MIN 151
        // V_MAX 256


        // green light
        // setHSVmin(Scalar(68, 126, 91));
        // setHSVmax(Scalar(90, 256, 256));
        // H_MIN 68
        // H_MAX 90
        // S_MIN 126
        // S_MAX 256
        // V_MIN 91
        // V_MAX 256


        //BGR value for Yellow:
        setColour(Scalar(256, 0, 0));
    }
}

Sphero::~Sphero(void)
{
}

int Sphero::getXPos()
{

    return Sphero::xPos;

}

void Sphero::setXPos(int x)
{

    Sphero::xPos = x;

}

int Sphero::getYPos()
{

    return Sphero::yPos;

}

void Sphero::setYPos(int y)
{

    Sphero::yPos = y;

}

Scalar Sphero::getHSVmin()
{

    return Sphero::HSVmin;

}
Scalar Sphero::getHSVmax()
{

    return Sphero::HSVmax;
}

void Sphero::setHSVmin(Scalar min)
{

    Sphero::HSVmin = min;
}


void Sphero::setHSVmax(Scalar max)
{

    Sphero::HSVmax = max;
}