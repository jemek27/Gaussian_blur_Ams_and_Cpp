.data
    n      equ 14                       ; Sta?a rozmiaru tablicy
    one_double REAL8 1.0                ; Definicja zmiennej double 1.0

    testTabX REAL8 16 dup(0.0)          ; Tablica typu double na wyniki exp(x)
    testTabN DWORD 16 dup(0)            ; Tablica int na silnie
    x REAL8 2.7                         ; Parametr x dla expAsm
    xf dd 2.7                           ; Parametr x dla expAsm
    two REAL8 2.0                       ; Warto?? 2.0 do mno?enia
    pi      dd 3.14159265358979323846   ; Stała Pi jako wartość typu float (32-bit)
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
; createGaussianKernel does not save the states of the registers. Possible overwriting
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

    addss xmm3, xmm0           ; sum += G(x)

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

;//////////////////////////////////
    ; Wejście: 
    ;   - rcx: bitmapData (wskazanie na dane bitmapy)
    ;   - rdx: array args [width, height, stride, kernelSize]
    ;   - r8: *tempData
    ;   - r9; *kernel
 ;///////////////  
    ; Uzywane rejestry:
    ; rax   tempVal
    ; rbx   tempVal, pixelIndex -> selectedIndex
    ; rcx   x
    ; rdx   y
    ; r8    width   
    ; r9    height
    ; r10   stride
    ; r11   kernelSize
    ; r12   pixelIndex
    ; r13   i (kernel index)
    ; r14   offset = kernelSize / 2; // we consider the middle index as 0 e.g. [-2, -1, 0, 1, 2]
    ; r15   tempVal
    ; rsi   *bitmapData
    ; rdi   *kernel
    ;
    ; 
    ;
    ; xmm0 = xxxx rBlr gBlr bBlr  
    ; xmm1 = xxxx rImg gImg bImg 
    ; xmm3 = kerI kerI kerI kerI 
gaussBlurAsm proc
; call createGaussianKernel does not modify r11-15
    xor  r11, r11
    mov  r15, rcx                       ; rcx needed for func call
    mov  r14, rdx                       ; rdx needed for func call
    mov  r11d, dword ptr [rdx + 12]     ; r11 = kernelSize
    mov  rdi, r9                        ; rdi = *kernel

                

    ; imul  rax, r11, 4            ; rax = kernelSize * 4 (flaot = 4 bytes)
    ;     ret
    ; sub   rsp, rax               ; allock array of size kernelSize on stack

    ; mov     r8, r9
    ; mov     rcx, r11            ; cl = kernelSize
    ; movss   xmm1, xmm2          ; xmm1 = sigma
    
    ; xmm1 - sigma (flaot)
    ; R8 - pointer to tabK (float array)
    ; CL - kernelSize (Byte)
    ; call createGaussianKernel 
    ; ret
    mov  rdx, r14               ; restoring parameter  
    mov  rsi, r15               ; rsi = *bitmapData
    mov  r15, r8                        ; r15 = *tempData

    ; lea rsp, [r8]               ; rsp has 

    ; imul  rax, r11, 4        
    ; add   rsp, rax  
    ; lea rsp, [r8]             
    ; ret

    ; Prepare parameters
    xor  r8, r8   
    xor  r9, r9 
    xor  r10, r10 


    mov  r8d, dword ptr [rdx]            ; r8  = width   
    mov  r9d, dword ptr [rdx + 4]        ; r9  = height
    mov  r10d, dword ptr [rdx + 8]       ; r10 = stride
    ; mov  r11d, dword ptr [rdx + 12]      ; r11 = kernelSize


    mov  r14, r11                        ; kernelSize -> r14
    shr  r14, 1                          ; r14 = offset = kernelSize / 2 

; Y-loop: Goes through half the height
    xor  rcx, rcx                ; rcx -> y = 0
y_loop:
    cmp  rcx, r9                 ; if (y >= height), end loop
    jge  end_y_loop

; X-loop: Goes through half the width
    xor  rdx, rdx                ; rdx -> x = 0
x_loop:
    cmp  rdx, r8                 ; if (x >= width), end loop
    jge  end_x_loop
    ; TODO może być mul zmainst imul
    ; Oblicz index piksela (pixelIndex = y * stride + x * 3)
    mov   r12, rcx               ; r12 = y
    imul  r12, r10               ; r12 = y * stride
    imul  rbx, rdx, 3            ; rbx = x * 3
    add   r12, rbx               ; r12 = pixelIndex = y * stride + x * 3    

    ; I-loop: goes through kernel
    ; xmm0 = xxxx rBlr gBlr bBlr  
    xorps xmm0, xmm0    ; blurredPixels (only 3 so 1 will be ignorred)
    xor   r13, r13      ; i
i_loop:
    cmp  r13, r11                ; if (i >= kernelSize), zakończ pętlę I ->  (i < kernelSize)
    jge  end_i_loop
    mov  rbx, r13                ; rbx = i;
    sub  rbx, r14                ; rbx = i - offset
    mov  rax, rbx                ; (i - offset) will reuse in a moment
    add  rbx, rdx                ; rbx = (i - offset) + x
    

    cmp  rbx, 0                  ; if (selectedX < 0) end -> (selectedX >= 0)
    jl   i_skip                  ; &&
    cmp  rbx, r8                 ; if (selectedX >= width) end -> (selectedX < width)
    jge  i_skip
    
    imul rax, 3                 ;((i - offset) * 3)
    mov  rbx, r12               ; rbx = pixelIndex
    add  rbx, rax               ; rbx = selectedIndex = pixelIndex + ((i - offset) * 3)

    ; get bytes
    ; xmm3 = kerI kerI kerI kerI 
    vbroadcastss xmm3, dword ptr [rdi + r13 * 4]      ; Załaduj jedną wartość float (kernel[i]) i umieść ją we wszystkich 4 slotach xmm0
    
    ; xmm1 = xxxx rImg gImg bImg 
    movzx ebx, byte ptr [rsi + rbx]     ; Załaduj i rozszerz byte1 na 32-bitowy rejestr eax
    cvtsi2ss xmm1, ebx                  ; Konwertuj eax (32-bitowy int) na float i umieść w xmm1

    movzx ebx, byte ptr [rsi + rbx + 1]     
    cvtsi2ss xmm2, ebx 
    insertps xmm1, xmm2, 16             ; Wstaw float z xmm2 do xmm1 w drugim elemencie (offset 0x10)

    
    movzx ebx, byte ptr [rsi + rbx + 2]     
    cvtsi2ss xmm2, ebx  
    insertps xmm1, xmm2, 32 

    mulps xmm1, xmm3                    ; xmm1 = xmm1 * xmm3 (mnoży wszystkie elementy w pakietach)
    addps xmm0, xmm1                    ; xmm0 = xmm0 + xmm1 

i_skip:
    inc r13                        ; i++
    jmp i_loop                     ; powrót do pętli i


    

end_i_loop:
    ; save blured pixel
    ; 1. Extract 1st value from xmm0
    cvttss2si eax, xmm0                 ; Konwersja float -> int
    mov byte ptr [r15 + r12], al        ; Zapisz wynik jako bajt

    ; 2. Extract 2nd value from xmm0
    psrldq xmm0, 4                      ; byte shift [3,2,1,0] --> [X,3,2,1]
    cvttss2si eax, xmm0     
    mov byte ptr [r15 + r12 + 1], al 

    ; 3. Extract 3rd value from xmm0
    psrldq xmm0, 4                      ; byte shift [X,3,2,1] --> [X,X,3,2]
    cvttss2si eax, xmm0     
    mov byte ptr [r15 + r12 + 2], al 

    inc rdx                        ; x++
    jmp x_loop                     ; powrót do pętli X

end_x_loop:
    inc rcx                        ; y++
    jmp y_loop                     ; powrót do pętli Y

end_y_loop:

;///////////////////////////////

; Y-loop: Goes through half the height
    xor  rcx, rcx                ; rcx -> y = 0
y_loop_2nd:
    cmp  rcx, r9                 ; if (y >= height), end loop
    jge  end_y_loop_2nd

; X-loop: Goes through half the width
    xor  rdx, rdx                ; rdx -> x = 0
x_loop_2nd:
    cmp  rdx, r8                 ; if (x >= width), end loop
    jge  end_x_loop_2nd
    ; TODO może być mul zmainst imul
    ; Oblicz index piksela (pixelIndex = y * stride + x * 3)
    mov   r12, rcx               ; r12 = y
    imul  r12, r10               ; r12 = y * stride
    imul  rbx, rdx, 3            ; rbx = x * 3
    add   r12, rbx               ; r12 = pixelIndex = y * stride + x * 3    

    ; I-loop: goes through kernel
    ; xmm0 = xxxx rBlr gBlr bBlr  
    xorps xmm0, xmm0    ; blurredPixels (only 3 so 1 will be ignorred)
    xor   r13, r13      ; i
i_loop_2nd: ;TODO x y swap
    cmp  r13, r11                ; if (i >= kernelSize), zakończ pętlę I ->  (i < kernelSize)
    jge  end_i_loop_2nd
    mov  rbx, r13                ; rbx = i;
    sub  rbx, r14                ; rbx = i - offset
    mov  rax, rbx                ; (i - offset) will reuse in a moment
    add  rbx, rcx                ; rbx = (i - offset) + y
    

    cmp  rbx, 0                  ; if (selectedY < 0) end -> (selectedY >= 0)
    jl   i_skip_2nd              ; &&
    cmp  rbx, r9                 ; if (selectedY >= height) end -> (selectedY < height)
    jge  i_skip_2nd
    
    imul rax, r10               ; ((i - offset) * stride)
    mov  rbx, r12               ; rbx = pixelIndex
    add  rbx, rax               ; rbx = selectedIndex = pixelIndex + ((i - offset) * stride)

    ; get bytes
    ; xmm3 = kerI kerI kerI kerI 
    vbroadcastss xmm3, dword ptr [rdi + r13 * 4]      ; Załaduj jedną wartość float (kernel[i]) i umieść ją we wszystkich 4 slotach xmm0
    
    ; xmm1 = xxxx rImg gImg bImg 
    movzx ebx, byte ptr [r15 + rbx]     ; Załaduj i rozszerz byte1 na 32-bitowy rejestr eax
    cvtsi2ss xmm1, ebx                  ; Konwertuj eax (32-bitowy int) na float i umieść w xmm1

    movzx ebx, byte ptr [r15 + rbx + 1]     
    cvtsi2ss xmm2, ebx 
    insertps xmm1, xmm2, 16             ; Wstaw float z xmm2 do xmm1 w drugim elemencie (offset 0x10)

    
    movzx ebx, byte ptr [r15 + rbx + 2]     
    cvtsi2ss xmm2, ebx  
    insertps xmm1, xmm2, 32 

    mulps xmm1, xmm3                    ; xmm1 = xmm1 * xmm3 (mnoży wszystkie elementy w pakietach)
    addps xmm0, xmm1                    ; xmm0 = xmm0 + xmm1 

i_skip_2nd:
    inc r13                        ; i++
    jmp i_loop_2nd                     ; powrót do pętli i


    

end_i_loop_2nd:
    ; save blured pixel
    ; 1. Extract 1st value from xmm0
    cvttss2si eax, xmm0                 ; Konwersja float -> int
    mov byte ptr [rsi + r12], al        ; Zapisz wynik jako bajt

    ; 2. Extract 2nd value from xmm0
    psrldq xmm0, 4                      ; byte shift [3,2,1,0] --> [X,3,2,1]
    cvttss2si eax, xmm0     
    mov byte ptr [rsi + r12 + 1], al 

    ; 3. Extract 3rd value from xmm0
    psrldq xmm0, 4                      ; byte shift [X,3,2,1] --> [X,X,3,2]
    cvttss2si eax, xmm0     
    mov byte ptr [rsi + r12 + 2], al 

    inc rdx                        ; x++
    jmp x_loop_2nd                     ; powrót do pętli X

end_x_loop_2nd:
    inc rcx                        ; y++
    jmp y_loop_2nd                     ; powrót do pętli Y

end_y_loop_2nd:
    ; free memeory 
    ; imul  rax, r11, 4        
    ; add  rsp, rax               
    ; ret
    ret
gaussBlurAsm endp
;//////////////////////////////////

;//////////////////////////////////
ProcessBitmapAsm proc
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
ProcessBitmapAsm endp
;//////////////////////////////////
end