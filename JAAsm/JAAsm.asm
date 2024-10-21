.data
    n      equ 14               ; Sta?a rozmiaru tablicy
    one_double REAL8 1.0        ; Definicja zmiennej double 1.0

    testTabX REAL8 16 dup(0.0)  ; Tablica typu double na wyniki exp(x)
    testTabN DWORD 16 dup(0)    ; Tablica int na silnie
    x REAL8 2.7                 ; Parametr x dla expAsm
    xf dd 2.7                 ; Parametr x dla expAsm
    two REAL8 2.0               ; Warto?? 2.0 do mno?enia
    pi      dd 3.14159265358979323846  ; Stała Pi jako wartość typu float (32-bit)
    one     dd 1.0 
    neg_one     dd -1.0 
.code
; xmm0 - x (float)
; RDX - pointer to tabX (double array)
; R8  - pointer to tabN (int array)
expAsm proc
    ; Zapisz stan rejestrów ogólnego przeznaczenia
    push rbx
    push rax
    push rdi
    push rsi

    ; Zapisz stan rejestrów zmiennoprzecinkowych (XMM0 - XMM2)
    sub rsp, 32                  ; Zarezerwuj miejsce na 2 rej 128-bitowe
    movdqu [rsp], xmm1           ; Zapisz xmm0
    movdqu [rsp+16], xmm2        ; Zapisz xmm1

    ; Operacje na tabX - Obliczanie pot?g x (e^x)
    lea rdi, [testTabX]         ; Wczytaj wska?nik na tabX do rejestru RDI
    xor rbx, rbx                ; Ustaw RBX (indeks) na 0
    cvtss2sd xmm0, xmm0         ; Konwertuj float na double (xmm0)
    movsd xmm1, xmm0            ; Przechowaj x w xmm1
    movsd xmm0, [one_double]    ; Za?aduj 1.0 do xmm0 (double)
    movsd qword ptr [rdi], xmm0 ; Zapisz warto?? double w pami?ci tabX[0]

    ; Pot?gowanie (e^x) z uwzgl?dnieniem silni
x_powers:
    cmp rbx, n                              ; Sprawd?, czy RBX (indeks) nie przekracza rozmiaru tablicy
    je enf_of_x_powers                      ; Je?eli tak, zako?cz p?tl?
    inc rbx                                 ; Zwi?ksz indeks
    mulsd xmm0, xmm1                        ; Pomn�? xmm0 przez x (x = RCX)
    ; Wstawienie warto?ci do tabX
    movsd qword ptr [rdi + rbx * 8], xmm0   ; Zapisz wynik do tabX[rbx]
    jmp x_powers                            ; Powr�? do p?tli

enf_of_x_powers:
    ; Operacje na tabN - Obliczanie n!
    lea rsi, [testTabN]             ; Wczytaj wska?nik na tabN do rejestru RSI
    mov dword ptr [rsi], 1          ; tabN[0] = 1
    xor rbx, rbx                    ; Ustaw RBX (indeks) na 0
    mov rax, 1                      ;rax = 1 (pierwszy element)

n_factorial:
    cmp rbx, n                          ; Sprawd?, czy RBX (indeks) nie przekracza rozmiaru tablicy
    je enf_of_n_factorial               ; Je?eli tak, zako?cz p?tl?
    inc rbx 
    imul rax, rbx                       ; Pomn�? rax przez 1, 2, 3 ...
    mov dword ptr [rsi + rbx * 4], eax  ; Zapisz wynik do tablicy tabN[rbx]
    jmp n_factorial                     ; Powr�? do p?tli

enf_of_n_factorial:
    xor rbx, rbx            ; Counter
    pxor xmm2, xmm2         ; Wyzeruj xmm2 (suma)

sum_loop:
    ; Operacja sum += tabX[i] / tabN[i]
    cmp rbx, n              ; Sprawd? czy RBX (indeks) nie przekracza rozmiaru tablicy
    je end_sum              ; Je?eli tak, zako?cz p?tl?

    movsd xmm0, qword ptr [rdi + rbx * 8]       ; Za?aduj tabX[i] do xmm0
    cvtsi2sd xmm1, dword ptr [rsi + rbx * 4]    ; Za?aduj tabN[i] i konwertuj na double
    divsd xmm0, xmm1                            ; xmm0 = tabX[i] / double(tabN[i])

    addsd xmm2, xmm0                            ; Dodaj wynik dzielenia do sumy (w xmm2)

    inc rbx                                     ; Zwi?ksz indeks
    jmp sum_loop                                ; Powr�? do p?tli

end_sum:
    movsd xmm0, xmm2            ; Przenie? sum? do xmm0
    cvtsd2ss xmm0, xmm0         ; konwertuj double z powrotem na float

    ; Przywracanie stanu rejestrów
    movdqu xmm1, [rsp]           ; Przywróć xmm0
    movdqu xmm2, [rsp+16]        ; Przywróć xmm1
    add rsp, 32                  ; Zwolnij miejsce na stosie

    pop rsi                      ; Przywróć stan rejestru RSI
    pop rdi                      ; Przywróć stan rejestru RDI
    pop rax                      ; Przywróć stan rejestru RAX
    pop rbx                      ; Przywróć stan rejestru RBX

    ret
expAsm endp

;//////////////////////////////////

;//////////////////////////////////
MyProc1 proc
add RCX, RDX
mov RAX, RCX
ret
MyProc1 endp

; xmm1 - sigma (flaot)
; R8 - pointer to tabK (float array)
; CL - kernelSize (Byte)
createGaussianKernel proc
    lea rdi, [r8]              ; Za?aduj wska?nik tablicy kernel do rejestru rdi

    movzx rax, cl              ; Przekonwertuj kernelSize (byte) na 64-bitow? warto?? (rax)
    mov rcx, rax
    xor rbx, rbx                ; Ustaw indeks na 0

    mulss xmm1, xmm1           ; sigma^2
    movss xmm6, xmm1          ; xmm6 = sigma^2
    addss xmm6, xmm6           ; xmm6 = 2 * sigma^2

    xorps xmm3, xmm3           ; Zero sum (xmm3) - będzie przechowywać sumę wyników

    ; Ustawienie indeksu x na kernelSize / -2
    sar rax, 1                 ; Podziel przez 2, aby uzyskać -kernelSize / 2
    neg rax                    ; Zrób -kernelSize w rax
    cvtsi2ss xmm4, rax         ; Załaduj x (indeks) do xmm4 jako float

    movss xmm10, [one]       ; Załaduj wartość 1.0 do xmm0

fill_kernel_loop:
    ; Oblicz Gauss 1D G(x) = exp(-x^2 / (2 * sigma^2)) / sqrt(2 * M_PI * sigma^2)
    ; Oblicz x^2
    movss xmm5, xmm4
    mulss xmm5, xmm5           ; xmm5 = x^2

    ; Oblicz -x^2 / (2 * sigma^2)
    divss xmm5, xmm6           ; xmm5 = x^2 / (2 * sigma^2)

    movss xmm0, [neg_one]
    mulss xmm5, xmm0           ; xmm5 = -x^2 / (2 * sigma^2)
 
    ; Oblicz exp(-x^2 / (2 * sigma^2))
    movss xmm0, xmm5


    call expAsm                   ; wywołaj funkcję exp, wynik w xmm0

    ; Oblicz sqrt(2 * M_PI * sigma^2)
    movss xmm8, dword ptr [pi] ; Załaduj wartość M_PI
    mulss xmm8, xmm6           ; xmm8 = PI * (2 * sigma^2) 
    sqrtss xmm8, xmm8         ; xmm8 = sqrt(2 * sigma^2 * M_PI)

    ; Oblicz G(x) = exp(-x^2 / (2 * sigma^2)) / sqrt(2 * M_PI * sigma^2)
    divss xmm0, xmm8           ; xmm0 = G(x)

    ; Wstaw wynik do tablicy kernel[i]
    
    movss dword ptr [rdi + rbx * 4], xmm0 ; Wpisz wynik do kernel[rcx]

    ; Zaktualizuj sumę
    addss xmm3, xmm0           ; sum += G(x)

    ; Zwiększ x
    addss xmm4, xmm10        ; xmm4 + 1
    inc rbx
    cmp rbx, rcx
    jl fill_kernel_loop        ; Jeżeli i < kernelSize, powróć do pętli


    xor rbx, rbx                ; Ustaw indeks na 0
    ; ret
normalize_kernel_loop:
    ; Normalizuj kernel
    movss xmm0, dword ptr [rdi + rbx * 4]
    divss xmm0, xmm3           ; kernel[i] /= sum
    movss dword ptr [rdi + rbx * 4], xmm0

    inc rbx
    cmp rbx, rcx
    jl normalize_kernel_loop

    ret
createGaussianKernel endp
;//////////////////////////////////

gaussBlurAsm proc
    ; Wejście: 
    ;   - rcx: bitmapData (wskazanie na dane bitmapy)
    ;   - rdx: width (szerokość obrazu)
    ;   - r8:  height (wysokość obrazu)
    ;   - r9:  stride (ilość bajtów na linię skanowania)    

    ; Zapisz stan rejestrów, które będziesz modyfikować
    ;push rbx
    ;push rsi
    ;push rdi

    ; Przygotuj parametry
    mov rsi, rcx        ; bitmapData -> rsi
    mov r10, r8         ; height -> r10
    shr r10, 1          ; height / 2

    ; Y-loop: Przechodzi przez połowę wysokości
    xor rbx, rbx               ; rbx -> y = 0
y_loop:
    cmp rbx, r8               ; if (y >= height), zakończ pętlę
    jge end_y_loop

    ; X-loop: Przechodzi przez połowę szerokości
    xor rcx, rcx               ; rcx -> x = 0
x_loop:
    cmp rcx, rdx               ; if (x >= width), zakończ pętlę X
    jge end_x_loop

    ; Oblicz index piksela (pixelIndex = y * stride + x * 3)
    mov rax, rbx                ; rax = y
    imul rax, r9                ; rax = y * stride
    imul r11, rcx, 3            ; r11 = x * 3
    add  rax, r11               ; rax = y * stride + x * 3      

    ; Sprawdzenie, czy jesteśmy w górnej połowie obrazu
    cmp rbx, r10
    jl set_white               ; Jeśli y < height / 2, ustaw biały kolor

    ; Ustaw czerwony kolor (dolna połowa)
    mov byte ptr [rsi + rax], 0     ; Blue = 0
    mov byte ptr [rsi + rax + 1], 0 ; Green = 0
    mov byte ptr [rsi + rax + 2], 255; Red = 255
    jmp continue_x_loop

set_white:
    ; Ustaw biały kolor (górna połowa)
    mov byte ptr [rsi + rax], 255   ; Blue = 255
    mov byte ptr [rsi + rax + 1], 255; Green = 255
    mov byte ptr [rsi + rax + 2], 255; Red = 255

continue_x_loop:
    inc rcx                        ; x++
    jmp x_loop                     ; powrót do pętli X

end_x_loop:
    inc rbx                        ; y++
    jmp y_loop                     ; powrót do pętli Y

end_y_loop:
    ; Przywróć stan rejestrów
    ;pop rdi
    ;pop rsi
    ;pop rbx

    ret
gaussBlurAsm endp
;//////////////////////////////////
end