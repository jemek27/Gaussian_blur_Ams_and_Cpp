.data
    n           equ 14                          ; Array size constant
    one_double  REAL8 1.0                       ; Definition of double variable 1.0

    testTabX    REAL8 16 dup(0.0)               ; Double array for exp(x) results
    testTabN    DWORD 16 dup(0)                 ; Int array for factorial
    x           REAL8 2.7                       ; Parameter x for expAsm
    xf          dd 2.7                          ; Parameter x for expAsm
    two         REAL8 2.0                       ; Value 2.0 for multiplication
    pi          dd 3.14159265358979323846       ; Constant Pi as a float (32-bit)
    one         dd 1.0 
    neg_one     dd -1.0 
.code
; xmm0 - x (float)
; RDX - pointer to tabX (double array)
; R8  - pointer to tabN (int array)
expAsm proc
; Save the state of general purpose registers
    push rbx
    push rax
    push rdi
    push rsi


; Save floating point registers (XMM0 - XMM2)
    sub rsp, 32                  ; Reserve space for 2 128-bit registers
    movdqu [rsp], xmm1           ; Save xmm0
    movdqu [rsp+16], xmm2        ; Save xmm1


; TabX operations - Calculating powers of x (e^x)
    lea rdi, [testTabX]         ; Load the pointer to tabX into the RDI register
    xor rbx, rbx                ; Set RBX (index) to 0
    cvtss2sd xmm0, xmm0         ; Convert float to double (xmm0)
    movsd xmm1, xmm0            ; Store x in xmm1
    movsd xmm0, [one_double]    ; Load 1.0 into xmm0 (double)
    movsd qword ptr [rdi], xmm0 ; Store the double value in tabX[0]

; Exponentiation (e^x) with factorial
x_powers:
    cmp rbx, n                              ; Check if RBX (index) does not exceed the size of the array
    je enf_of_x_powers                      ; If so, end the loop
    inc rbx                                 ; Increment the index
    mulsd xmm0, xmm1                        ; Multiply xmm0 by x (x = RCX)
; Insert the value into tabX
    movsd qword ptr [rdi + rbx * 8], xmm0   ; Write the result to tabX[rbx]
    jmp x_powers                            ; loop back

enf_of_x_powers:
; TabN operations - Calculating n!
    lea rsi, [testTabN]             ; Read the tabN pointer into the RSI register
    mov dword ptr [rsi], 1          ; tabN[0] = 1
    xor rbx, rbx                    ; Set RBX (index) to 0
    mov rax, 1                      ; rax = 1 (first element)

n_factorial:
    cmp rbx, n                          ; Check if RBX (index) does not exceed the size of the array
    je enf_of_n_factorial               ; If so, exit the loop
    inc rbx 
    imul rax, rbx                       ; Multiply rax by 1, 2, 3 ...
    mov dword ptr [rsi + rbx * 4], eax  ; Store the result in the array tabN[rbx]
    jmp n_factorial                     ; loop back

enf_of_n_factorial:
    xor rbx, rbx            ; Counter
    pxor xmm2, xmm2         ; Reset xmm2 (sum)

sum_loop:
; Operation sum += tab[i] / tab[i] 
    cmp rbx, n              ; Check if RBX (index) does not exceed the size of the array
    je end_sum              ; If so, end the loop

    movsd xmm0, qword ptr [rdi + rbx * 8]       ; Load tabX[i] into xmm0
    cvtsi2sd xmm1, dword ptr [rsi + rbx * 4]    ; Load tabN[i] and convert to double
    divsd xmm0, xmm1                            ; xmm0 = tabX[i] / double(tabN[i])

    addsd xmm2, xmm0                            ; Add the result of division to the sum (in xmm2)

    inc rbx                                     ; ++index
    jmp sum_loop                                ; loop back

end_sum:
    movsd xmm0, xmm2             ; Move sum to xmm0
    cvtsd2ss xmm0, xmm0          ; convert double back to float

    ; Restore registers
    movdqu xmm1, [rsp]           ; Restore xmm0
    movdqu xmm2, [rsp+16]        ; Restore xmm1
    add rsp, 32                  ; Free stack space

    pop rsi                      ; Restore RSI register
    pop rdi                      ; Restore RDI register
    pop rax                      ; Restore RAX register
    pop rbx                      ; Restore RBX register

    ret
expAsm endp

;//////////////////////////////////

;//////////////////////////////////
MyProc1 proc
add RCX, RDX
mov RAX, RCX
ret
MyProc1 endp
; createGaussianKernelAsm does not save the states of the registers. Possible overwriting
; xmm1 - sigma (flaot)
; R8 - pointer to tabK (float array)
; CL - kernelSize (Byte)
createGaussianKernelAsm proc


    lea rdi, [r8]           ; Load the kernel array pointer into the rdi register 

    movzx rax, cl           ; Convert kernelSize (byte) to 64-bit?// TODO chceck if needed
    mov rcx, rax
    xor rbx, rbx            ; Set the index to 0

    mulss xmm1, xmm1        ; sigma^2
    movss xmm6, xmm1        ; xmm6 = sigma^2
    addss xmm6, xmm6        ; xmm6 = 2 * sigma^2

    xorps xmm3, xmm3        ; Zero sum (xmm3) - will store the sum of the results




    ; Set index x to kernelSize / -2
    sar rax, 1              ; Divide by 2 to get -kernelSize / 2
    neg rax                 ; Make -kernelSize in rax
    cvtsi2ss xmm4, rax      ; Load x (index) into xmm4 as float

    movss xmm10, [one]      ; Load value 1.0 into xmm0

fill_kernel_loop:
; Calculate Gauss 1D G(x) = exp(-x^2 / (2 * sigma^2)) / sqrt(2 * M_PI * sigma^2)
; Calculate x^2
    movss xmm5, xmm4
    mulss xmm5, xmm5           ; xmm5 = x^2

    ; -x^2 / (2 * sigma^2)
    divss xmm5, xmm6           ; xmm5 = x^2 / (2 * sigma^2)

    movss xmm0, [neg_one]
    mulss xmm5, xmm0           ; xmm5 = -x^2 / (2 * sigma^2)
 
    ; exp(-x^2 / (2 * sigma^2))
    movss xmm0, xmm5


    call expAsm                   ; call exp function, result in xmm0

    ; sqrt(2 * M_PI * sigma^2)
    movss xmm8, dword ptr [pi]  ; Load M_PI value
    mulss xmm8, xmm6            ; xmm8 = M_PI * (2 * sigma^2) 
    sqrtss xmm8, xmm8           ; xmm8 = sqrt(2 * sigma^2 * M_PI)

    ; G(x) = exp(-x^2 / (2 * sigma^2)) / sqrt(2 * M_PI * sigma^2)
    divss xmm0, xmm8           ; xmm0 = G(x)

; Insert the result into the kernel[i] array 
    movss dword ptr [rdi + rbx * 4], xmm0 ; Write the result to kernel[rcx]

    addss xmm3, xmm0           ; sum += G(x)

    addss xmm4, xmm10        ; xmm4 + 1
    inc rbx
    cmp rbx, rcx
    jl fill_kernel_loop        ; If i < kernelSize, loop back


    xor rbx, rbx                ; Set index as 0

normalize_kernel_loop:
; Normalize kernel
    movss xmm0, dword ptr [rdi + rbx * 4]
    divss xmm0, xmm3           ; kernel[i] /= sum
    movss dword ptr [rdi + rbx * 4], xmm0

    inc rbx
    cmp rbx, rcx
    jl normalize_kernel_loop

    ret
createGaussianKernelAsm endp
;//////////////////////////////////

;//////////////////////////////////
    ; Wejście: 
    ;   - rcx: bitmapData (wskazanie na dane bitmapy)
    ;   - rdx: array args [width, height, stride, kernelSize]
    ;   - r8: *tempData
    ;   - r9; *kernel
 ;///////////////  
    ; Uzywane rejestry:
    ; rax   tempVal, selectedIndex
    ; rbx   tempVal, pixelIndex 
    ; rcx   x
    ; rdx   y
    ; r8    width   
    ; r9    height
    ; r10   stride
    ; r11   kernelSize
    ; r12   pixelIndex
    ; r13   i (kernel index)
    ; r14   offset = kernelSize / 2; // we consider the middle index as 0 e.g. [-2, -1, 0, 1, 2]
    ; r15   *tempData
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
    ; call createGaussianKernelAsm 
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
    ; TODO may be mul instead of imul?
    ; Calculate pixel index (pixelIndex = y * stride + x * 3)
    mov   r12, rcx               ; r12 = y
    imul  r12, r10               ; r12 = y * stride
    imul  rbx, rdx, 3            ; rbx = x * 3
    add   r12, rbx               ; r12 = pixelIndex = y * stride + x * 3    

    ; I-loop: goes through kernel
    ; xmm0 = xxxx rBlr gBlr bBlr  
    xorps xmm0, xmm0    ; blurredPixels (only 3 so 1 will be ignorred)
    xor   r13, r13      ; i
i_loop:
    cmp  r13, r11                ; if (i >= kernelSize), end I loop ->  (i < kernelSize)
    jge  end_i_loop
    mov  rbx, r13                ; rbx = i;
    sub  rbx, r14                ; rbx = i - offset
    mov  rax, rbx                ; (i - offset) will reuse in a moment line 303
    add  rbx, rdx                ; rbx = selectedX = (i - offset) + x
    

    cmp  rbx, 0                  ; if (selectedX < 0) end -> (selectedX >= 0)
    jl   less_than_zero          ; &&
    cmp  rbx, r8                 ; if (selectedX >= width) end -> (selectedX < width)
    jge  more_than_width
    
    imul rax, 3                 ;((i - offset) * 3)
    mov  rbx, r12               ; rbx = pixelIndex
    add  rax, rbx               ; rax = selectedIndex = pixelIndex + ((i - offset) * 3)

    jmp continue                ; skip next code fragment (represents other case)

;if out of border we want to mirror the edge 
;-2 -1 [ 0 1 ... n-1 n ] n+1 n+2     
; 1  0 [ 0 1 ... n-1 n ] n   n-1    

less_than_zero:
    add  rbx, 1                 ; 1 + selectedX
    imul rbx, 3                 ; (1 + selectedX) * 3
    neg  rbx                    ; (1 + selectedX) * -3
    mov  rax, r12               ; rax = pixelIndex
    add  rax, rbx               ;selectedIndex += (1 + selectedX) * -3;

    jmp continue                ; skip next code fragment (represents other case)

more_than_width:
    add  rbx, 1                 ; 1 + selectedX
    mov  rax, r8                ; rax = width
    sub  rax, rbx               ; rax = width - 1 - selectedX = width - (1 + selectedX)
    imul rax, 3                 ; rax = (width - 1 - selectedX) * 3
    add  rax, r12               ; rax = selectedIndex += (width - 1 - selectedX) * 3;

continue: 
    ; get bytes
    ; xmm3 = kerI kerI kerI kerI 
    vbroadcastss xmm3, dword ptr [rdi + r13 * 4]      ; Load one float value (kernel[i]) and put it in all 4 xmm0 slots
    
    ; xmm1 = xxxx rImg gImg bImg 
    movzx ebx, byte ptr [rsi + rax]     ; Load and expand byte1 into the 32-bit eax register
    cvtsi2ss xmm1, ebx                  ; Convert eax (32bit int) to float and put it in xmm1

    movzx ebx, byte ptr [rsi + rax + 1]     
    cvtsi2ss xmm2, ebx 
    insertps xmm1, xmm2, 16             ; Insert float from xmm2 into xmm1 in second slot (offset 0x10)

    
    movzx ebx, byte ptr [rsi + rax + 2]     
    cvtsi2ss xmm2, ebx  
    insertps xmm1, xmm2, 32 

    mulps xmm1, xmm3                    ; xmm1 = xmm1 * xmm3 (multiplies all elements in the packets)
    addps xmm0, xmm1                    ; xmm0 = xmm0 + xmm1 


    inc r13                        ; i++
    jmp i_loop                     ; back to I loop


    

end_i_loop:
    ; save blured pixel in temp
    ; 1. Extract 1st value from xmm0
    cvttss2si eax, xmm0                 ; Conversion float -> int
    mov byte ptr [r15 + r12], al        ; Save the result as a byte

    ; 2. Extract 2nd value from xmm0
    psrldq xmm0, 4                      ; byte shift [3,2,1,0] --> [X,3,2,1]
    cvttss2si eax, xmm0     
    mov byte ptr [r15 + r12 + 1], al 

    ; 3. Extract 3rd value from xmm0
    psrldq xmm0, 4                      ; byte shift [X,3,2,1] --> [X,X,3,2]
    cvttss2si eax, xmm0     
    mov byte ptr [r15 + r12 + 2], al 

    inc rdx                        ; x++
    jmp x_loop                     ; back to X loop

end_x_loop:
    inc rcx                        ; y++
    jmp y_loop                     ; back to Y loop

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
    ; TODO may be mul instead of imul?
    ; Calculate pixel index (pixelIndex = y * stride + x * 3)
    mov   r12, rcx               ; r12 = y
    imul  r12, r10               ; r12 = y * stride
    imul  rbx, rdx, 3            ; rbx = x * 3
    add   r12, rbx               ; r12 = pixelIndex = y * stride + x * 3    

    ; I-loop: goes through kernel
    ; xmm0 = xxxx rBlr gBlr bBlr  
    xorps xmm0, xmm0    ; blurredPixels (only 3 so 1 will be ignorred)
    xor   r13, r13      ; i
i_loop_2nd: ;TODO x y swap
    cmp  r13, r11                ; if (i >= kernelSize), end I loop ->  (i < kernelSize)
    jge  end_i_loop_2nd
    mov  rbx, r13                ; rbx = i;
    sub  rbx, r14                ; rbx = i - offset
    mov  rax, rbx                ; (i - offset) will reuse in a moment
    add  rbx, rcx                ; rbx = (i - offset) + y
    

    cmp  rbx, 0                  ; if (selectedY < 0) end -> (selectedY >= 0)
    jl   less_than_zero_2nd      ; &&
    cmp  rbx, r9                 ; if (selectedY >= height) end -> (selectedY < height)
    jge  more_than_height_2nd
    
    imul rax, r10               ; ((i - offset) * stride)
    mov  rbx, r12               ; rbx = pixelIndex
    add  rax, rbx               ; rbx = selectedIndex = pixelIndex + ((i - offset) * stride)

    jmp continue_2nd               ; skip next code fragment (represents other case)
;TODO tu tyż przerobić
less_than_zero_2nd:
    add  rbx, 1                 ; 1 + selectedY
    imul rbx, r10               ; (1 + selectedY) * stride
    neg  rbx                    ; (1 + selectedY) * -stride
    mov  rax, r12               ; rax = pixelIndex
    add  rax, rbx               ;selectedIndex += (1 + selectedX) * -stride;

    jmp continue_2nd                ; skip next code fragment (represents other case)

more_than_height_2nd:

    add  rbx, 1                 ; 1 + selectedY
    mov  rax, r9                ; rax = height
    sub  rax, rbx               ; rax = height - 1 - selectedY = height - (1 + selectedy)
    imul rax, r10               ; rax = (height - 1 - selectedY) * stride
    add  rax, r12               ; rax = selectedIndex += (width - 1 - selectedY) * stride;

continue_2nd:  
    ; get bytes
    ; xmm3 = kerI kerI kerI kerI 
    vbroadcastss xmm3, dword ptr [rdi + r13 * 4]      ; Load one float value (kernel[i]) and put it in all 4 xmm0 slots
    
    ; xmm1 = xxxx rTImg gTImg bTImg 
    movzx ebx, byte ptr [r15 + rax]     ; Load and expand byte1 into the 32-bit eax register
    cvtsi2ss xmm1, ebx                  ; Convert eax (32bit int) to float and put it in xmm1

    movzx ebx, byte ptr [r15 + rax + 1]     
    cvtsi2ss xmm2, ebx 
    insertps xmm1, xmm2, 16             ; Insert float from xmm2 into xmm1 in second slot (offset 0x10)

    
    movzx ebx, byte ptr [r15 + rax + 2]     
    cvtsi2ss xmm2, ebx  
    insertps xmm1, xmm2, 32 

    mulps xmm1, xmm3                    ; xmm1 = xmm1 * xmm3 (multiplies all elements in the packets)
    addps xmm0, xmm1                    ; xmm0 = xmm0 + xmm1 

i_skip_2nd:
    inc r13                        ; i++
    jmp i_loop_2nd                 ; back to I loop


    

end_i_loop_2nd:
    ; save blured pixel in Img
    ; 1. Extract 1st value from xmm0
    cvttss2si eax, xmm0                 ; Conversion float -> int
    mov byte ptr [rsi + r12], al        ; Save the result as a byte

    ; 2. Extract 2nd value from xmm0
    psrldq xmm0, 4                      ; byte shift [3,2,1,0] --> [X,3,2,1]
    cvttss2si eax, xmm0     
    mov byte ptr [rsi + r12 + 1], al 

    ; 3. Extract 3rd value from xmm0
    psrldq xmm0, 4                      ; byte shift [X,3,2,1] --> [X,X,3,2]
    cvttss2si eax, xmm0     
    mov byte ptr [rsi + r12 + 2], al 

    inc rdx                        ; x++
    jmp x_loop_2nd                 ; back to X loop

end_x_loop_2nd:
    inc rcx                        ; y++
    jmp y_loop_2nd                 ; back to Y loop

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