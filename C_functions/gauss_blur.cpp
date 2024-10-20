#include "gauss_blur.h"
#include "pch.h"
#include <cmath>


# define M_PI   3.14159265358979323846

//TODO Remember to delete the array after usage
double* createGaussianKernel(unsigned __int8 kernelSize, float sigma) {

    if (kernelSize & 0b00000001) { //is uneven
        double* kernel = new double[kernelSize];
        int x = kernelSize / -2; // we consider the middle index as 0 e.g. [-2, -1, 0, 1, 2]
        
        double sigmaPower2 = pow(sigma, 2);
        double sum = 0; //for subsequent normalisation

        for (int i = 0; i < kernelSize; ++i) {
            // Gauss 1D G(x)
            kernel[i] = exp(pow(x++, 2) / ( -2 * sigmaPower2) / (sqrt(2 * M_PI * sigmaPower2)) );
            sum += kernel[i];
        }

        for (int i = 0; i < kernelSize; ++i) {
            kernel[i] /= sum;
        }

        return kernel;
    } else {
        return nullptr;
    }
}

//TODO co z czarn¹ ramk¹? Spr w ps i porównaæ. Jak dobrac odpowiedni krenel wedle rozmiaru obrazu?
extern "C" __declspec(dllexport) void gaussBlur(unsigned char* bitmapData, int width, int height, int stride, int kernelSize, float sigma) {
    double* kernel = createGaussianKernel(kernelSize, sigma);
    if (!kernel) { return; }

    int numOfPixels = width * height * 3; //3 bytes per pixel(BGR format)
    unsigned char* tempData = new unsigned char[numOfPixels];
    if (!tempData) {
        delete[] kernel;
        return;
    }

    int offset = kernelSize / 2; // we consider the middle index as 0 e.g. [-2, -1, 0, 1, 2]

    // Horizontal blur
    for (int y = 0; y < height; ++y) {
        for (int x = 0; x < width; ++x) {
            int pixelIndex = y * stride + x * 3; // BMP uses 3 bytes per pixel (BGR format)

            double blurredPixelR = 0, blurredPixelG = 0, blurredPixelB = 0;

            for (int i = 0; i < kernelSize; ++i) {
                int selectedX = x + (i - offset);
                if (selectedX >= 0 && selectedX < width) {
                    int selectedIndex = pixelIndex + ((i - offset) * 3);
                    blurredPixelB += bitmapData[selectedIndex] * kernel[i];
                    blurredPixelG += bitmapData[selectedIndex + 1] * kernel[i];
                    blurredPixelR += bitmapData[selectedIndex + 2] * kernel[i];
                }
            }

            tempData[pixelIndex] = static_cast<unsigned char>(std::round(blurredPixelB));
            tempData[pixelIndex + 1] = static_cast<unsigned char>(std::round(blurredPixelG));
            tempData[pixelIndex + 2] = static_cast<unsigned char>(std::round(blurredPixelR));
        }
    }
    
    // Vertical blur
    for (int y = 0; y < height; ++y) {
        for (int x = 0; x < width; ++x) {
            int pixelIndex = y * stride + x * 3;

            double blurredPixelR = 0, blurredPixelG = 0, blurredPixelB = 0;

            for (int i = 0; i < kernelSize; ++i) {
                int selectedY = y + (i - offset);
                if (selectedY >= 0 && selectedY < height) {
                    int selectedIndex = pixelIndex + ((i - offset) * stride);
                    blurredPixelB += tempData[selectedIndex] * kernel[i];
                    blurredPixelG += tempData[selectedIndex + 1] * kernel[i];
                    blurredPixelR += tempData[selectedIndex + 2] * kernel[i];
                }
            }

            bitmapData[pixelIndex] = static_cast<unsigned char>(std::round(blurredPixelB));
            bitmapData[pixelIndex + 1] = static_cast<unsigned char>(std::round(blurredPixelG));
            bitmapData[pixelIndex + 2] = static_cast<unsigned char>(std::round(blurredPixelR));
        }
    }

    delete[] tempData;
    delete[] kernel;
}


extern "C" __declspec(dllexport) void ProcessBitmap(unsigned char* bitmapData, int width, int height, int stride) {

    for (int y = 0; y < height / 2; ++y) {
        for (int x = 0; x < width / 2; ++x) {
            int pixelIndex = y * stride + x * 3; // BMP uses 3 bytes per pixel (BGR format)

            if (y < height / 2) {
                // Top half to white (255, 255, 255)
                bitmapData[pixelIndex + 0] = 255; // Blue
                bitmapData[pixelIndex + 1] = 255; // Green
                bitmapData[pixelIndex + 2] = 255; // Red
            }
            else {
                // Bottom half to red (0, 0, 255)
                bitmapData[pixelIndex + 0] = 0;   // Blue
                bitmapData[pixelIndex + 1] = 0;   // Green
                bitmapData[pixelIndex + 2] = 255; // Red
            }
        }
    }
}
