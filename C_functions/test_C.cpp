#include "test_C.h"
#include "pch.h"

extern "C" __declspec(dllexport) int Add(int a, int b) {
    return a + b;
}
