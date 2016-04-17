/**
 * @file CannyDetector_Demo.cpp
 * @brief Sample code showing how to detect edges using the Canny Detector
 * @author OpenCV team
 */

#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/highgui/highgui.hpp"
#include <stdlib.h>
#include <stdio.h>
#include <iostream>
#include "Sphero.h"
#include <netinet/in.h>

using namespace cv;

/// Global variables

const int FRAME_WIDTH = 1280;
const int FRAME_HEIGHT = 720;

int H_MIN = 0;
int H_MAX = 256;
int S_MIN = 0;
int S_MAX = 256;
int V_MIN = 0;
int V_MAX = 256;
const int MAX_NUM_OBJECTS = 2;
// minimum and maximum object area
const int MIN_OBJECT_AREA = 10 * 10;
const int MAX_OBJECT_AREA = 40 * 40;
const char* trackbar_name = "Trackbars";
const string window_name = "Original";
const string window_name_1 = "HSV";
const string window_name_2 = "Thresholded";
const string window_name_3 = "After Morphological Operations";
void on_trackbar( int, void* )
{
    printf("\n H_MIN %d \n", H_MIN);
    printf("H_MAX %d \n", H_MAX);
    printf("S_MIN %d \n", S_MIN);
    printf("S_MAX %d \n", S_MAX);
    printf("V_MIN %d \n", V_MIN);
    printf("V_MAX %d \n", V_MAX);
}

void createTrackbars()
{
    //create window for trackbars

    char TrackbarName[50];
    namedWindow(trackbar_name, 0);

    createTrackbar( "H_MIN", trackbar_name, &H_MIN, H_MAX, on_trackbar );
    createTrackbar( "H_MAX", trackbar_name, &H_MAX, H_MAX, on_trackbar );
    createTrackbar( "S_MIN", trackbar_name, &S_MIN, S_MAX, on_trackbar );
    createTrackbar( "S_MAX", trackbar_name, &S_MAX, S_MAX, on_trackbar );
    createTrackbar( "V_MIN", trackbar_name, &V_MIN, V_MAX, on_trackbar );
    createTrackbar( "V_MAX", trackbar_name, &V_MAX, V_MAX, on_trackbar );
    // createTrackbar( "H_MIN", trackbar_name, &H_MIN, 163, on_trackbar );
    // createTrackbar( "H_MAX", trackbar_name, &H_MAX, 242, on_trackbar );
    // createTrackbar( "S_MIN", trackbar_name, &S_MIN, 94, on_trackbar );
    // createTrackbar( "S_MAX", trackbar_name, &S_MAX, 256, on_trackbar );
    // createTrackbar( "V_MIN", trackbar_name, &V_MIN, 0, on_trackbar );
    // createTrackbar( "V_MAX", trackbar_name, &V_MAX, 252, on_trackbar );


}
string intToString(int number)
{


    std::stringstream ss;
    ss << number;
    return ss.str();
}

void morphOps(Mat &thresh)
{
    Mat erodeElement = getStructuringElement( MORPH_RECT, Size(3, 3));
    Mat dilateElement = getStructuringElement( MORPH_RECT, Size(8, 8));
    erode(thresh, thresh, erodeElement);
    erode(thresh, thresh, erodeElement);
    dilate(thresh, thresh, dilateElement);
    dilate(thresh, thresh, dilateElement);
}
void drawObject(vector<Sphero> spheros, Mat &frame)
{
    for (int i = 0; i < spheros.size(); i++) {

        cv::circle(frame, cv::Point(spheros.at(i).getXPos(), spheros.at(i).getYPos()), 10, cv::Scalar(0, 0, 255));
        cv::putText(frame, intToString(spheros.at(i).getXPos()) + " , " + intToString(spheros.at(i).getYPos()), cv::Point(spheros.at(i).getXPos(), spheros.at(i).getYPos() + 20), 1, 1, Scalar(0, 255, 0));
        cv::putText(frame, spheros.at(i).getType(), cv::Point(spheros.at(i).getXPos(), spheros.at(i).getYPos() - 30), 1, 2, spheros.at(i).getColour());
    }
}

void trackFilteredObject(Sphero &theSphero, Mat threshold, Mat HSV, Mat &cameraFeed)
{


    vector <Sphero> spheros;

    Mat temp;
    threshold.copyTo(temp);
    //these two vectors needed for output of findContours
    vector< vector<Point> > contours;
    vector<Vec4i> hierarchy;
    //find contours of filtered image using openCV findContours function
    findContours(temp, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
    //use moments method to find our filtered object
    double refArea = 0;
    bool objectFound = false;
    if (hierarchy.size() > 0) {
        int numObjects = hierarchy.size();
        //if number of objects greater than MAX_NUM_OBJECTS we have a noisy filter
        if (numObjects < MAX_NUM_OBJECTS) {
            for (int index = 0; index >= 0; index = hierarchy[index][0]) {

                Moments moment = moments((cv::Mat)contours[index]);
                double area = moment.m00;

                //if the area is less than 20 px by 20px then it is probably just noise
                //if the area is the same as the 3/2 of the image size, probably just a bad filter
                //we only want the object with the largest area so we safe a reference area each
                //iteration and compare it to the area in the next iteration.
                if (area > MIN_OBJECT_AREA) {
                    // Sphero boo;

                    theSphero.setXPos(moment.m10 / area);
                    theSphero.setYPos(moment.m01 / area);
                    // boo.setType(theSphero.getType());
                    // boo.setColour(theSphero.getColour());

                    spheros.push_back(theSphero);

                    objectFound = true;

                } else objectFound = false;


            }
            //let user know you found an object
            if (objectFound == true) {
                //draw object location on screen
                drawObject(spheros, cameraFeed);
            }

        } else putText(cameraFeed, "TOO MUCH NOISE! ADJUST FILTER", Point(0, 50), 1, 2, Scalar(0, 0, 255), 2);
    }
    if (!objectFound) {
        theSphero.setXPos(-1);
        theSphero.setYPos(-1);
    }
}

void trackFilteredObject(Mat threshold, Mat HSV, Mat &cameraFeed)
{
    vector <Sphero> spheros;

    Mat temp;
    threshold.copyTo(temp);
    //these two vectors needed for output of findContours
    vector< vector<Point> > contours;
    vector<Vec4i> hierarchy;
    //find contours of filtered image using openCV findContours function
    findContours(temp, contours, hierarchy, CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE );
    //use moments method to find our filtered object
    double refArea = 0;
    bool objectFound = false;
    if (hierarchy.size() > 0) {
        int numObjects = hierarchy.size();
        if (numObjects < MAX_NUM_OBJECTS) {
            for (int index = 0; index >= 0; index = hierarchy[index][0]) {

                Moments moment = moments((cv::Mat)contours[index]);
                double area = moment.m00;
                if (area > MIN_OBJECT_AREA) {

                    Sphero boo;

                    boo.setXPos(moment.m10 / area);
                    boo.setYPos(moment.m01 / area);


                    spheros.push_back(boo);

                    objectFound = true;

                } else objectFound = false;


            }
            //let user know you found an object
            if (objectFound == true) {
                //draw object location on screen
                drawObject(spheros, cameraFeed);
            }

        } else putText(cameraFeed, "TOO MUCH NOISE! ADJUST FILTER", Point(0, 50), 1, 2, Scalar(0, 0, 255), 2);
    }
}


int setupServerConnection(sockaddr_in *servaddr, int *fd)
{
    /* our address */
    // int fd;  our socket
    unsigned int alen;  /* length of address (for getsockname) */

    // create         ipv4     udp                   socket
    if ((*fd = socket(AF_INET, SOCK_DGRAM, 0)) < 0) {
        perror("cannot create socket");
        return 0;
    }
    printf("created socket: %d\n", *fd);

    memset((void *)servaddr, 0, sizeof(*servaddr));
    servaddr->sin_family = AF_INET;
    servaddr->sin_addr.s_addr = htonl(INADDR_ANY);
    // set port
    servaddr->sin_port = htons(1337);

    // if (bind(*fd, (struct sockaddr *)servaddr, sizeof(*servaddr)) < 0) {
    //     perror("bind failed");
    //     return 0;
    // }

    printf("bind complete. Port number = %d\n", ntohs(servaddr->sin_port));
}


typedef struct {
    int x;
    int y;
    int id;
} spheroLoc;

int sendToServer(int *fd, sockaddr_in *servaddr, Sphero sphero, int id)
{
    char message[100];
    sprintf(message, "%d,%d,%d", id, sphero.getXPos(), sphero.getYPos());

    printf("id: %d, x: %d, y: %d\n", id, sphero.getXPos(), sphero.getYPos());

    // send a message to the server
    if (sendto(*fd, message, strlen(message), 0, (struct sockaddr *)servaddr, sizeof(*servaddr)) < 0) {
        perror("sendto failed");
        return 0;
    }
}

#define DEBUG

int main( int, char** argv )
{
    Mat frame;
    Mat threshold;
    Mat HSV;
    VideoCapture capture;
    bool calibrationMode = false;

    //open capture object at location zero (default location for webcam)
    capture.open(1);
    // capture.open("/Users/jamus/dev/rollout/spherocontrol/cvsphero/SpheroDetector/test5.mov");

    if (!capture.isOpened())
        return -1;
    // capture.open(0);
    //set height and width of capture frame
    capture.set(CV_CAP_PROP_FRAME_WIDTH, FRAME_WIDTH);
    capture.set(CV_CAP_PROP_FRAME_HEIGHT, FRAME_HEIGHT);
    // namedWindow( window_name, WINDOW_AUTOSIZE );

    struct sockaddr_in serverAddress;
    int socketDescriptor;
    spheroLoc sphero1 = {0, 0, 0};

    setupServerConnection(&serverAddress, &socketDescriptor);

    while (true) {
        if (capture.read(frame) == NULL) {
            printf("!!! cvQueryFrame failed: no frame\n");
            capture.set(CV_CAP_PROP_POS_AVI_RATIO , 0);
            continue;
        }

        if (calibrationMode) {
            cvtColor(frame, HSV, COLOR_BGR2HSV);
            inRange(HSV, Scalar(H_MIN, S_MIN, V_MIN), Scalar(H_MAX, S_MAX, V_MAX), threshold);
            morphOps(threshold);
            imshow(window_name_2, threshold);
            createTrackbars();
            trackFilteredObject(threshold, HSV, frame);
        } else {
            Sphero boo("boo");
            cvtColor(frame, HSV, COLOR_BGR2HSV);
            inRange(HSV, boo.getHSVmin(), boo.getHSVmax(), threshold);
            morphOps(threshold);
            trackFilteredObject(boo, threshold, HSV, frame);

            // imshow(window_name, threshold);

            Sphero ybr("ybr");
            cvtColor(frame, HSV, COLOR_BGR2HSV);
            inRange(HSV, ybr.getHSVmin(), ybr.getHSVmax(), threshold);
            morphOps(threshold);
            trackFilteredObject(ybr, threshold, HSV, frame);

            sendToServer(&socketDescriptor, &serverAddress, ybr, 0);
            sendToServer(&socketDescriptor, &serverAddress, boo, 1);

            imshow(window_name, frame);
        }

        if (waitKey(25) == 27)  break;
    }
    capture.release();
    return 0;
}