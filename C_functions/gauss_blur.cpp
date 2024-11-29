#include "gauss_blur.h"
#include "pch.h"
#include <cmath>


# define M_PI   3.14159265358979323846

extern "C" __declspec(dllexport) void createGaussianKernel(unsigned __int8 kernelSize, float sigma, float* kernel) {

    if (kernelSize & 0b00000001) { //is uneven
        int x = kernelSize / -2; // we consider the middle index as 0 e.g. [-2, -1, 0, 1, 2]

        float s = 2 * sigma * sigma;
        float sum = 0; //for subsequent normalisation

        for (int i = 0; i < kernelSize; ++i) {
            // Gauss 1D G(x)
            kernel[i] = exp( (- 1 * (x * x)) / s ) / (sqrt(M_PI * s));
            sum += kernel[i];
            ++x;
        }

        for (int i = 0; i < kernelSize; ++i) {
            kernel[i] /= sum;
        }
    }
}

//           wait until all threads finish working \/                    |>>>>>>>>>>..> output data
//    gaussBlurStage1(bitmapData, tempData, [...])   gaussBlurStage2(bitmapData, tempData, [...])
// input data >>>>>>>>>>> ^          |>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> ^

extern "C" __declspec(dllexport) void gaussBlurStage1(unsigned char* bitmapData, unsigned char* tempData, float* kernel,
    int width, int height, int stride, int kernelSize,
    int startHeight, int endHeight) {

    int offset = kernelSize / 2; // we consider the middle index as 0 e.g. [-2, -1, 0, 1, 2]

    // Horizontal blur
    for (int y = startHeight; y < endHeight; ++y) {
        for (int x = 0; x < width; ++x) {
            int pixelIndex = y * stride + x * 3; // BMP uses 3 bytes per pixel (BGR format)

            float blurredPixelR = 0, blurredPixelG = 0, blurredPixelB = 0;

            for (int i = 0; i < kernelSize; ++i) {
                int selectedX = x + (i - offset);
                int selectedIndex = pixelIndex;

                //if out of border we want to mirror the edge 
                //-2 -1 [ 0 1 ... n-1 n ] n+1 n+2     
                // 1  0 [ 0 1 ... n-1 n ] n   n-1    
                if (selectedX < 0) {
                    selectedIndex += (1 + selectedX) * -3; //(1 + selectedX) * -1 * 3;
                }
                else if (selectedX >= width) {
                    selectedIndex += (width - 1 - selectedX) * 3;
                }
                else {
                    selectedIndex += ((i - offset) * 3);
                }

                blurredPixelB += bitmapData[selectedIndex] * kernel[i];
                blurredPixelG += bitmapData[selectedIndex + 1] * kernel[i];
                blurredPixelR += bitmapData[selectedIndex + 2] * kernel[i];
            }

            tempData[pixelIndex] = static_cast<unsigned char>(std::round(blurredPixelB));
            tempData[pixelIndex + 1] = static_cast<unsigned char>(std::round(blurredPixelG));
            tempData[pixelIndex + 2] = static_cast<unsigned char>(std::round(blurredPixelR));
        }
    }
}

extern "C" __declspec(dllexport) void gaussBlurStage2(unsigned char* bitmapData, unsigned char* tempData, float* kernel,
    int width, int height, int stride, int kernelSize,
    int startHeight, int endHeight) {

    int offset = kernelSize / 2; // we consider the middle index as 0 e.g. [-2, -1, 0, 1, 2]

    // Vertical blur
    for (int y = startHeight; y < endHeight; ++y) {
        for (int x = 0; x < width; ++x) {
            int pixelIndex = y * stride + x * 3;


            float blurredPixelR = 0, blurredPixelG = 0, blurredPixelB = 0;

            for (int i = 0; i < kernelSize; ++i) {
                int selectedY = y + (i - offset);
                int selectedIndex = pixelIndex;

                //if out of border we want to mirror the edge 
                //-2 -1 [ 0 1 ... n-1 n ] n+1 n+2     
                // 1  0 [ 0 1 ... n-1 n ] n   n-1    
                if (selectedY < 0) {
                    selectedIndex += (1 + selectedY) * -1 * stride; //(1 + selectedX) * -1 * 3;
                }
                else if (selectedY >= height) {
                    selectedIndex += (height - 1 - selectedY) * stride;
                }
                else {
                    selectedIndex += ((i - offset) * stride);
                }


                blurredPixelB += tempData[selectedIndex] * kernel[i];
                blurredPixelG += tempData[selectedIndex + 1] * kernel[i];
                blurredPixelR += tempData[selectedIndex + 2] * kernel[i];
            }

            bitmapData[pixelIndex] = static_cast<unsigned char>(std::round(blurredPixelB));
            bitmapData[pixelIndex + 1] = static_cast<unsigned char>(std::round(blurredPixelG));
            bitmapData[pixelIndex + 2] = static_cast<unsigned char>(std::round(blurredPixelR));
        }
    }
}

//combines both stages
extern "C" __declspec(dllexport) void gaussBlur(unsigned char* bitmapData, unsigned char* tempData, float* kernel,
    int width, int height, int stride, int kernelSize) {

    int offset = kernelSize / 2; // we consider the middle index as 0 e.g. [-2, -1, 0, 1, 2]

    // Horizontal blur
    for (int y = 0; y < height; ++y) {
        for (int x = 0; x < width; ++x) {
            int pixelIndex = y * stride + x * 3; // BMP uses 3 bytes per pixel (BGR format)

            float blurredPixelR = 0, blurredPixelG = 0, blurredPixelB = 0;

            for (int i = 0; i < kernelSize; ++i) {
                int selectedX = x + (i - offset);
                int selectedIndex = pixelIndex;

                //if out of border we want to mirror the edge 
                //-2 -1 [ 0 1 ... n-1 n ] n+1 n+2     
                // 1  0 [ 0 1 ... n-1 n ] n   n-1    
                if (selectedX < 0) {
                    selectedIndex += (1 + selectedX) * -3; //(1 + selectedX) * -1 * 3;
                }
                else if (selectedX >= width) {
                    selectedIndex += (width - 1 - selectedX) * 3;
                }
                else {
                    selectedIndex += ((i - offset) * 3);
                }

                blurredPixelB += bitmapData[selectedIndex] * kernel[i];
                blurredPixelG += bitmapData[selectedIndex + 1] * kernel[i];
                blurredPixelR += bitmapData[selectedIndex + 2] * kernel[i];
            }

            tempData[pixelIndex]     = static_cast<unsigned char>(std::round(blurredPixelB));
            tempData[pixelIndex + 1] = static_cast<unsigned char>(std::round(blurredPixelG));
            tempData[pixelIndex + 2] = static_cast<unsigned char>(std::round(blurredPixelR));
        }
    }

    // Vertical blur
    for (int y = 0; y < height; ++y) {
        for (int x = 0; x < width; ++x) {
            int pixelIndex = y * stride + x * 3;


            float blurredPixelR = 0, blurredPixelG = 0, blurredPixelB = 0;

            for (int i = 0; i < kernelSize; ++i) {
                int selectedY = y + (i - offset);
                int selectedIndex = pixelIndex;

                //if out of border we want to mirror the edge 
                //-2 -1 [ 0 1 ... n-1 n ] n+1 n+2     
                // 1  0 [ 0 1 ... n-1 n ] n   n-1    
                if (selectedY < 0) {
                    selectedIndex += (1 + selectedY) * -1 * stride; //(1 + selectedX) * -1 * 3;
                }
                else if (selectedY >= height) {
                    selectedIndex += (height - 1 - selectedY) * stride;
                }
                else {
                    selectedIndex += ((i - offset) * stride);
                }


                blurredPixelB += tempData[selectedIndex] * kernel[i];
                blurredPixelG += tempData[selectedIndex + 1] * kernel[i];
                blurredPixelR += tempData[selectedIndex + 2] * kernel[i];
            }

            bitmapData[pixelIndex]      = static_cast<unsigned char>(std::round(blurredPixelB));
            bitmapData[pixelIndex + 1]  = static_cast<unsigned char>(std::round(blurredPixelG));
            bitmapData[pixelIndex + 2]  = static_cast<unsigned char>(std::round(blurredPixelR));
        }
    }
}