/**
 * @file CannyDetector_Demo.cpp
 * @brief Sample code showing how to detect edges using the Canny Detector
 * @author OpenCV team
 */

#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/highgui/highgui.hpp"
#include <stdlib.h>
#include <stdio.h>

using namespace cv;

/// Global variables

Mat frame, dst, detected_edges;
Mat hsvImg, blueImg;
vector<Vec3f> detectedcircles;

int cannyThresh = 1 ;
int houghThresh = 1 ;
int const max_cannyThresh = 100;
int const max_houghThresh = 100;
int const max_kernelSize = 100;
int ratio = 3;
int kernelSize = 1;

const char* window_name = "Hough";

/**
 * @function CannyThreshold
 * @brief Trackbar callback - Canny thresholds input with a ratio 1:3
 */
static void CannyThreshold(int, void*)
{
    GaussianBlur(blueImg, blueImg, Size(kernelSize * 2 + 1, kernelSize * 2 + 1), 5, 5);
    Canny( detected_edges, detected_edges, cannyThresh, max_cannyThresh * ratio, 3);
    dst = Scalar::all(0);

    HoughCircles( blueImg,
                  detectedcircles,    // where to output the circles
                  CV_HOUGH_GRADIENT,
                  1,
                  blueImg.rows / 8,   // min dist between detected circles
                  cannyThresh,                 // edge threshold
                  houghThresh,                 // hough space threshold
                  0,
                  0);

    for (size_t i = 0; i < detectedcircles.size(); i++) {
        Point center(cvRound(detectedcircles[i][0]),
                     cvRound(detectedcircles[i][1]));
        int radius = cvRound(detectedcircles[i][2]);
        circle( blueImg, center, 3, Scalar(0, 255, 0), -1, 8, 0 );
        circle( blueImg, center, radius, Scalar(0, 0, 255), 3, 8, 0 );
    }
//  frame.copyTo( dst, detected_edges);
    imshow( window_name, blueImg );
    /// Reduce noise with a kernel 3x3

    /// Canny detector

    // imshow( window_name, hsvImg );

}


/**
 * @function main
 */
int main( int, char** argv )
{
    VideoCapture camera(0);
    if (!camera.isOpened())
        return -1;

    namedWindow( window_name, WINDOW_AUTOSIZE );
    createTrackbar( "Min Canny Threshold:", window_name, &cannyThresh, max_cannyThresh, CannyThreshold );
    createTrackbar( "Min Hough Threshold:", window_name, &houghThresh, max_houghThresh, CannyThreshold );
    createTrackbar( "Min Kernel Size Threshold:", window_name, &kernelSize, max_kernelSize, CannyThreshold );

    while (true) {
        camera >> frame;
        cvtColor(frame, hsvImg, CV_BGR2HSV);
        inRange(hsvImg, cv::Scalar(0, 150, 100), cv::Scalar(20, 255, 255), blueImg);
        CannyThreshold(0, 0);
        if (waitKey(25) == 27)  break;
    }
    camera.release();
    return 0;
}