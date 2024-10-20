#pragma once

#include <comdef.h>

extern "C" __declspec(dllexport) void gaussBlur(unsigned char* bitmapData, int width, int height, int stride, int kernelSize, float sigma);
extern "C" __declspec(dllexport) void ProcessBitmap(unsigned char* bitmapData, int width, int height, int stride);
