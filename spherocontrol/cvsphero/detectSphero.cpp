#include <stdio.h>
#include <math.h>
#include "/usr/local/opencv-2.4/include/opencv2/opencv.hpp"
#include "/usr/local/opencv-2.4/include/opencv2/core/core.hpp"
#include "/usr/local/opencv-2.4/include/opencv2/highgui/highgui.hpp"

using namespace cv;

int main(int, char**)
{
    VideoCapture camera(0);
    if (!camera.isOpened())
        return -1;
    vector<Vec3f> detectedcircles;
    Mat frame,
        blueImg,
        hsvImg;

    namedWindow("Hough", 1);

    while (true) {
        camera >> frame;

        cvtColor(frame, hsvImg, CV_BGR2HSV);
        inRange(hsvImg, cv::Scalar(0, 150, 100), cv::Scalar(20, 255, 255), blueImg);

        GaussianBlur(blueImg, blueImg, Size(11, 11), 5, 5);

        HoughCircles( blueImg,
                      detectedcircles,    // where to output the circles
                      CV_HOUGH_GRADIENT,
                      1,
                      blueImg.rows / 8,   // min dist between detected circles
                      50,                // edge threshold
                      20,                 // hough space threshold
                      0,
                      0);

        for (size_t i = 0; i < detectedcircles.size(); i++) {
            Point center(cvRound(detectedcircles[i][0]),
                         cvRound(detectedcircles[i][1]));
            int radius = cvRound(detectedcircles[i][2]);
            circle( blueImg, center, 3, Scalar(0, 255, 0), -1, 8, 0 );
            circle( blueImg, center, radius, Scalar(0, 0, 255), 3, 8, 0 );
        }
        imshow("Hough Circles", blueImg);
        if (waitKey(25) == 27)  break; // esc to quit, waits 25ms ~= 40fps
    }
    camera.release();
    return 0;
}