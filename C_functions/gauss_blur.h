#pragma once

#include <comdef.h>

extern "C" __declspec(dllexport) void gaussBlur(unsigned char* bitmapData, unsigned char* tempData, double* kernel, 
												int width, int height, int stride, int kernelSize,
												int startHeight, int endHeight);
//extern "C" __declspec(dllexport) void gaussBlur(unsigned char* bitmapData, int width, int height, int stride, int kernelSize, float sigma);
extern "C" __declspec(dllexport) void ProcessBitmap(unsigned char* bitmapData, int width, int height, int stride);
//extern "C" __declspec(dllexport) double* createGaussianKernel(unsigned __int8 kernelSize, float sigma);
extern "C" __declspec(dllexport) void createGaussianKernel(unsigned __int8 kernelSize, float sigma, double* kernel);

extern "C" __declspec(dllexport) void gaussBlurStage1(	unsigned char* bitmapData, unsigned char* tempData, double* kernel,
														int width, int height, int stride, int kernelSize,
														int startHeight, int endHeight);

extern "C" __declspec(dllexport) void gaussBlurStage2(	unsigned char* bitmapData, unsigned char* tempData, double* kernel,
														int width, int height, int stride, int kernelSize,
														int startHeight, int endHeight);