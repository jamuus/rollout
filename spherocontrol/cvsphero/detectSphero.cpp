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
        blueImg;

    namedWindow("Hough", 1);

    while (true) {
        camera >> frame;

        cv::inRange(frame, cv::Scalar(30, 0, 0), cv::Scalar(255, 30, 30), blueImg);

        GaussianBlur( blueImg, blueImg, Size(15, 15), 10, 10 );

        HoughCircles( blueImg,
                      detectedcircles,    // where to output the circles
                      CV_HOUGH_GRADIENT,
                      1,
                      blueImg.rows / 8,   // min dist between detected circles
                      100,                // edge threshold
                      70,                 // hough space threshold
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
        if (waitKey(25) == 27)  break; // esc to quit
    }
    camera.release();
    return 0;
}