#include <stdio.h>
#include <math.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <string.h>

#ifdef __APPLE__
#   include "/usr/local/opencv-2.4/include/opencv2/opencv.hpp"
#   include "/usr/local/opencv-2.4/include/opencv2/core/core.hpp"
#   include "/usr/local/opencv-2.4/include/opencv2/highgui/highgui.hpp"
#else
#   include <opencv2/opencv.hpp>
#   include <opencv2/core/core.hpp>
#   include <opencv2/highgui/highgui.hpp>
#endif

using namespace cv;

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

    if (bind(*fd, (struct sockaddr *)servaddr, sizeof(*servaddr)) < 0) {
        perror("bind failed");
        return 0;
    }

    printf("bind complete. Port number = %d\n", ntohs(servaddr->sin_port));
}

typedef struct {
    int x;
    int y;
    int id;
} spheroLoc;

int sendToServer(int *fd, sockaddr_in *servaddr, spheroLoc loc)
{
    char message[100];
    sprintf(message, "%d,%d,%d", loc.id, loc.x, loc.y);

    // send a message to the server
    if (sendto(*fd, message, strlen(message), 0, (struct sockaddr *)servaddr, sizeof(*servaddr)) < 0) {
        perror("sendto failed");
        return 0;
    }
}

int main(int, char**)
{
    int testing = 1;
    VideoCapture camera(0);
    if (!camera.isOpened())
        return -1;
    vector<Vec3f> detectedOrangeCircles,
           detectedBlueCircles;
    Mat frame,
        orangeImage,
        blueImage,
        hsvImg,
        output;

    namedWindow("Hough", 1);

    struct sockaddr_in serverAddress;
    int socketDescriptor;
    spheroLoc sphero1 = {15, 50, 0};

    setupServerConnection(&serverAddress, &socketDescriptor);

    while (true) {
        if (testing) {
            sphero1.x = (sphero1.x + 1) % 100;
            sendToServer(&socketDescriptor, &serverAddress, sphero1);
            if (waitKey(25) == 27)  break;
        } else {
            camera >> frame;

            cvtColor(frame, hsvImg, CV_BGR2HSV);
            inRange(hsvImg, cv::Scalar(0, 150, 100), cv::Scalar(20, 255, 255), orangeImage);
            // needs adjusting
            inRange(hsvImg, cv::Scalar(128, 100, 100), cv::Scalar(192, 255, 255), blueImage);

            GaussianBlur(orangeImage, orangeImage, Size(11, 11), 5, 5);
            GaussianBlur(blueImage, blueImage, Size(11, 11), 5, 5);

            blueImage.copyTo(output);

            HoughCircles( orangeImage,
                          detectedOrangeCircles,        // where to output the circles
                          CV_HOUGH_GRADIENT,
                          1,
                          orangeImage.rows / 4,   // min dist between detected circles
                          50,                     // edge threshold
                          40,                     // hough space threshold
                          0,
                          0);

            HoughCircles( blueImage,
                          detectedBlueCircles,        // where to output the circles
                          CV_HOUGH_GRADIENT,
                          1,
                          blueImage.rows / 4,   // min dist between detected circles
                          50,                     // edge threshold
                          40,                     // hough space threshold
                          0,
                          0);

            for (size_t i = 0; i < detectedOrangeCircles.size(); i++) {
                Point center(cvRound(detectedOrangeCircles[i][0]),
                             cvRound(detectedOrangeCircles[i][1]));
                int radius = cvRound(detectedOrangeCircles[i][2]);
                circle( output, center, 3, Scalar(0, 255, 0), -1, 8, 0 );
                circle( output, center, radius, Scalar(0, 0, 255), 3, 8, 0 );
            }

            for (size_t i = 0; i < detectedBlueCircles.size(); i++) {
                Point center(cvRound(detectedBlueCircles[i][0]),
                             cvRound(detectedBlueCircles[i][1]));
                int radius = cvRound(detectedBlueCircles[i][2]);
                circle( output, center, 3, Scalar(0, 255, 0), -1, 8, 0 );
                circle( output, center, radius, Scalar(0, 0, 255), 3, 8, 0 );
            }

            imshow("Hough Circles", output);
            if (waitKey(25) == 27)  break;     // esc to quit, waits 25ms ~= 40fps
        }
    }
    camera.release();
    return 0;
}
