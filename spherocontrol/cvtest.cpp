#include <iostream>
#include <vector>

#include <opencv2/opencv.hpp>

int main()
{
    float width = 5.0f, height = 5.7f;

    std::vector<cv::Point2f> corners { { 0.0f, 0.0f }, { 4.0f, 1.0f }, { 3.4f, 15.0f }, { -0.5f, 13.7f } };
    std::vector<cv::Point2f> target { { 0, 0 }, { width, 0 }, { width, height }, { 0, height } };

    auto transform = cv::getPerspectiveTransform(corners, target);

    std::cout << transform << std::endl;

    cv::Point3f p(3.5f, 1.765f, 1.0f);

    std::vector<cv::Point3f> points { p };
    cv::transform(points, points, transform);

    std::cout << "\n" << points.front() << std::endl;

    return 0;
}
