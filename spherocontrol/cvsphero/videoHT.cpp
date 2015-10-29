#include <stdio.h>
#include <math.h>
#include "/usr/local/opencv-2.4/include/opencv2/opencv.hpp"
#include "/usr/local/opencv-2.4/include/opencv2/core/core.hpp"
#include "/usr/local/opencv-2.4/include/opencv2/highgui/highgui.hpp"

using namespace cv;

int main(int, char**)
{
    VideoCapture cap(0); 
    if(!cap.isOpened())  
        return -1;
    Mat gray;
    namedWindow("Hough",1);
// waitKey(0);
    for(;;)
    {

// waitKey(0);
    	vector<Vec3f> circles;
        Mat frame;
        // frame = imread("frame.jpg", CV_LOAD_IMAGE_COLOR);
   		cap >> frame;
        // cvtColor(frame, gray, CV_BGR2GRAY);
        // vector<Mat> channels(3);
        Mat blue;
// split img:
        // split(frame, channels);
        // blue = channels[0];
cv::inRange(frame, cv::Scalar(30, 0, 0), cv::Scalar(255, 30, 30), blue);
		GaussianBlur( blue, blue, Size(15,15), 10, 10 );
		HoughCircles( blue, circles, CV_HOUGH_GRADIENT, 1, blue.rows/8, 100, 70, 0, 0 );
	    for( size_t i = 0; i < circles.size(); i++ )
	    {
	        Point center(cvRound(circles[i][0]), cvRound(circles[i][1]));
	        int radius = cvRound(circles[i][2]);
	        circle( blue, center, 3, Scalar(0,255,0), -1, 8, 0 );
	        circle( blue, center, radius, Scalar(0,0,255), 3, 8, 0 );
	     }
       imshow("Hough Circles", blue);
        if(waitKey(25) == 27)  break;
    }
    cap.release();
    return 0;
}