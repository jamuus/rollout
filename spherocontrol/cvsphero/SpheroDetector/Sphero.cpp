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

        // for orange cover
        setHSVmin(Scalar(5, 175, 154));
        setHSVmax(Scalar(20, 256, 256));

        // for orange cover
        // setHSVmin(Scalar(0, 0, 131));
        // setHSVmax(Scalar(75, 256, 256));
        // H_MIN 0
        // H_MAX 75
        // S_MIN 0
        // S_MAX 256
        // V_MIN 131
        // V_MAX 256


        // for red sphero
        // setHSVmin(Scalar(0, 66, 45));
        // setHSVmax(Scalar(29, 180, 256));

        // for red sphero
        // setHSVmin(Scalar(163, 94, 0));
        // setHSVmax(Scalar(242, 256, 256));



        //BGR value for Orange:
        setColour(Scalar(0, 128, 256));
    }
    if (name == "ybr") {

        //TODO: use "calibration mode" to find HSV min
        //and HSV max values
        setType(name);
        setHSVmin(Scalar(98, 239, 126));
        setHSVmax(Scalar(112, 255, 256));

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