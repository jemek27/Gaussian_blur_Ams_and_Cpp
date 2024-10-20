#include <iostream>
#include <fstream>
#include <vector>
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
            kernel[i] = (1 / (sqrt(2 * M_PI * sigmaPower2))) * exp(pow(x++, 2) / (-2 * sigmaPower2));
            sum += kernel[i];
        }

        for (int i = 0; i < kernelSize; ++i) {
            kernel[i] /= sum;
        }

        return kernel;
    }
    else {
        return nullptr;
    }
}

extern "C" __declspec(dllexport) void gaussBlur(unsigned char* bitmapData, int width, int height, int stride, int kernelSize, float sigma) {
    double* kernel = createGaussianKernel(kernelSize, sigma);
    if (!kernel) { return; }

    for (int i = 0; i < kernelSize; ++i) {
        std::cout << kernel[i] << " | ";
    }
    std::cout << std::endl;

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

// Funkcja do wczytywania pliku BMP
bool loadBMP(const char* filename, std::vector<unsigned char>& bitmapData, int& width, int& height, int& stride) {
    std::ifstream file(filename, std::ios::binary);

    if (!file.is_open()) {
        std::cerr << "Nie udało się otworzyć pliku BMP!" << std::endl;
        return false;
    }

    unsigned char bmpHeader[54];
    file.read(reinterpret_cast<char*>(bmpHeader), 54);

    if (bmpHeader[0] != 'B' || bmpHeader[1] != 'M') {
        std::cerr << "To nie jest poprawny plik BMP!" << std::endl;
        return false;
    }

    // Wczytanie szerokości, wysokości i rozmiaru piksela
    width = *reinterpret_cast<int*>(&bmpHeader[18]);
    height = *reinterpret_cast<int*>(&bmpHeader[22]);
    int bitDepth = *reinterpret_cast<int*>(&bmpHeader[28]);
    stride = (width * bitDepth + 31) / 32 * 4; // Wyznaczenie linii wyrównania w BMP

    int dataSize = stride * height;
    bitmapData.resize(dataSize);

    file.read(reinterpret_cast<char*>(bitmapData.data()), dataSize);
    file.close();

    return true;
}

// Funkcja do zapisu zmodyfikowanego pliku BMP
bool saveBMP(const char* filename, const std::vector<unsigned char>& bitmapData, int width, int height, int stride) {
    std::ofstream file(filename, std::ios::binary);

    if (!file.is_open()) {
        std::cerr << "Nie udało się zapisać pliku BMP!" << std::endl;
        return false;
    }

    // Nagłówek BMP (prosty)
    unsigned char bmpHeader[54] = {
        'B', 'M',  // Magiczne znaki BMP
        0, 0, 0, 0,  // Rozmiar pliku - ustawimy później
        0, 0, 0, 0,  // Zarezerwowane
        54, 0, 0, 0,  // Offset danych
        40, 0, 0, 0,  // Rozmiar nagłówka DIB
        0, 0, 0, 0,  // Szerokość
        0, 0, 0, 0,  // Wysokość
        1, 0,        // Liczba płaszczyzn
        24, 0,       // Bit depth (24 bity)
        0, 0, 0, 0,  // Kompresja
        0, 0, 0, 0,  // Rozmiar obrazu - nie skompresowane
        0, 0, 0, 0,  // Rozdzielczość X
        0, 0, 0, 0,  // Rozdzielczość Y
        0, 0, 0, 0,  // Kolory w palecie
        0, 0, 0, 0   // Ważne kolory
    };

    int fileSize = 54 + stride * height;
    bmpHeader[2] = static_cast<unsigned char>(fileSize);
    bmpHeader[3] = static_cast<unsigned char>(fileSize >> 8);
    bmpHeader[4] = static_cast<unsigned char>(fileSize >> 16);
    bmpHeader[5] = static_cast<unsigned char>(fileSize >> 24);

    bmpHeader[18] = static_cast<unsigned char>(width);
    bmpHeader[19] = static_cast<unsigned char>(width >> 8);
    bmpHeader[20] = static_cast<unsigned char>(width >> 16);
    bmpHeader[21] = static_cast<unsigned char>(width >> 24);

    bmpHeader[22] = static_cast<unsigned char>(height);
    bmpHeader[23] = static_cast<unsigned char>(height >> 8);
    bmpHeader[24] = static_cast<unsigned char>(height >> 16);
    bmpHeader[25] = static_cast<unsigned char>(height >> 24);

    file.write(reinterpret_cast<char*>(bmpHeader), 54);
    file.write(reinterpret_cast<const char*>(bitmapData.data()), stride * height);
    file.close();

    return true;
}

int main() {
    // Wczytanie obrazu BMP
    std::vector<unsigned char> bitmapData;
    int width = 0, height = 0, stride = 0;

    if (!loadBMP("sum-ryba-900x450.bmp", bitmapData, width, height, stride)) {
        return 1;
    }

    std::cout << "Wczytano obraz o wymiarach: " << width << "x" << height << std::endl;

    // Parametry rozmycia Gaussa
    int kernelSize = 11;  // rozmiar jądra
    float sigma = 4.0f;  // sigma

    // Wykonanie rozmycia
    gaussBlur(bitmapData.data(), width, height, stride, kernelSize, sigma);

    // Zapis zmodyfikowanego obrazu
    if (!saveBMP("output.bmp", bitmapData, width, height, stride)) {
        return 1;
    }

    std::cout << "Zapisano przetworzony obraz jako output.bmp" << std::endl;

    return 0;
}
