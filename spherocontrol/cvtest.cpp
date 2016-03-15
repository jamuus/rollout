#include <iostream>
#include <vector>

#include <opencv2/opencv.hpp>

cv::Point2f warpPerspective(const cv::Point2f& p, const cv::Mat& m)
{
    cv::Point2f q(p.x * m.at<double>(0, 0) + p.y * m.at<double>(0, 1) + m.at<double>(0, 2),
        p.x * m.at<double>(1, 0) + p.y * m.at<double>(1, 1) + m.at<double>(1, 2));

    const double f = p.x * m.at<double>(2, 0) + p.y * m.at<double>(2, 1) + m.at<double>(2, 2);


    q.x /= f;
    q.y /= f;

    return q;
}

int main()
{
    float width = 5.0f, height = 5.0f;

    // std::vector<cv::Point2f> corners { { 0.0f, 0.0f }, { 4.0f, 1.0f }, { 3.4f, 15.0f }, { -0.5f, 13.7f } };
    std::vector<cv::Point2f> corners { { 0.0f, 0.0f }, { 4.0f, 0.0f }, { 4.0f, 4.0f }, { 0.0f, 4.0f } };
    std::vector<cv::Point2f> target { { 0, 0 }, { width, 0 }, { width, height }, { 0, height } };

    auto transform = cv::getPerspectiveTransform(corners, target);

    std::cout << transform << std::endl;

    cv::Point2f p(3.5f, 1.765f);

    p = warpPerspective(p, transform);

    std::cout << "\n" << p << std::endl;

    return 0;
}
