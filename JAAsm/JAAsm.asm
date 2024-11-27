.data
    n           equ 14                          ; Array size constant
    align 16
    one_double  REAL8 1.0                       ; Definition of double variable 1.0
    tabX    REAL8 16 dup(0.0)                   ; Double array for exp(x) results
    ; testTabN    DWORD 16 dup(0)                 ; Int array for factorial
    tabN QWORD 1.0, 2.0, 6.0, 24.0, 120.0, 720.0, 5040.0, 40320.0, 362880.0, 3628800.0, 39916800.0, 479001600.0, 6227020800.0, 87178291200.0, 1307674368000.0, 20922789888000.0

    two         REAL8 2.0                       ; Value 2.0 for multiplication
    pi          dd 3.14159265358979323846       ; Constant Pi as a float (32-bit)
    one         dd 1.0 
    neg_one     dd -1.0 
.code

;//////////////////////////////////
; Description:
    ; calculates exp(x)
; Input: 
    ; xmm0 - x (float)
expAsm proc
; Save the state of general purpose registers
    push rbx
    push rax
    push rdi
    push rsi

; Save floating point registers (XMM1 - XMM2)
    sub rsp, 32                  ; Reserve space for 2 128-bit registers
    movdqu [rsp], xmm1           ; Save xmm0
    movdqu [rsp+16], xmm2        ; Save xmm1

; TabX operations - Calculating powers of x (e^x)
    lea rdi, [tabX]         ; Load the pointer to tabX into the RDI register
    xor rbx, rbx                ; Set RBX (index) to 0
    cvtss2sd xmm0, xmm0         ; Convert float to double (xmm0)
    movsd xmm1, xmm0            ; Store x in xmm1

; Exponentiation (e^x) with factorial
x_powers:
    cmp rbx, n                              ; Check if RBX (index) does not exceed the size of the array
    je end_of_x_powers                      ; If so, end the loop
    movsd qword ptr [rdi + rbx * 8], xmm0   ; Write the result to tabX[rbx]

    mulsd xmm0, xmm1                        ; x^n * x
    inc rbx                                 ; Increment the index
    jmp x_powers                            ; loop back

end_of_x_powers:
    lea rsi, [tabN]             ; Read the tabN pointer into the RSI register

    xor rbx, rbx                ; Set RBX (index) to 0
    pxor xmm2, xmm2             ; Reset xmm2 (sum)
    movsd xmm2, [one_double]    ; Load 1.0 into xmm0 (double) (first elemet of Tylor expansion)

sum_loop:
; Operation sum += tab[i] / tab[i] 
    cmp rbx, n              ; Check if RBX (index) does not exceed the size of the array
    je end_sum              ; If so, end the loop

    movsd xmm0, qword ptr [rdi + rbx * 8]       ; Load tabX[i] into xmm0
    movsd xmm1, qword ptr [rsi + rbx * 8]       ; Load tabN[i] 
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
; Description:
    ; Generates a Gauss blur kernel of a specified size. 
    ; Given a kernel size and sigma value, the function calculates weights 
    ; that decrease with distance from the center, simulating the Gaussian function.
; Input: 
    ; xmm1 - sigma (flaot)
    ; CL - kernelSize (Byte)
    ; R8 - pointer to tabK (float array) result saved here
createGaussianKernelAsm proc
; Save callee-saved registers to stack
    push rbx
    push rcx
    push rdi

    lea rdi, [r8]           ; Load the kernel array pointer into the rdi register 

    xor rax, rax
    movzx rax, cl           ; Convert kernelSize (byte) to 64-bit?
    mov rcx, rax
    xor rbx, rbx            ; Set the index to 0

    mulss xmm1, xmm1        ; sigma^2
    movss xmm6, xmm1        ; xmm6 = sigma^2
    addss xmm6, xmm6        ; xmm6 = 2 * sigma^2

    ; sqrt(2 * M_PI * sigma^2)
    movss xmm8, dword ptr [pi]  ; Load M_PI value
    mulss xmm8, xmm6            ; xmm8 = M_PI * (2 * sigma^2) 
    sqrtss xmm8, xmm8           ; xmm8 = sqrt(2 * sigma^2 * M_PI)

    xorps xmm3, xmm3        ; Zero sum (xmm3) - will store the sum of the results
    xorps xmm4, xmm4        

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

; G(x) = exp(-x^2 / (2 * sigma^2)) / sqrt(2 * M_PI * sigma^2)
    divss xmm0, xmm8           ; xmm0 = G(x)

; Insert the result into the kernel[i] array 
    movss dword ptr [rdi + rbx * 4], xmm0 ; Write the result to kernel[rbx]

    addss xmm3, xmm0        ; sum += G(x)

    addss xmm4, xmm10       ; xmm4 + 1
    inc rbx
    cmp rbx, rcx
    jl fill_kernel_loop     ; If i < kernelSize, loop back

    xor rbx, rbx            ; Set index as 0
normalize_kernel_loop:
; Normalize kernel
    movss xmm0, dword ptr [rdi + rbx * 4]
    divss xmm0, xmm3           ; kernel[i] /= sum
    movss dword ptr [rdi + rbx * 4], xmm0

    inc rbx
    cmp rbx, rcx
    jl normalize_kernel_loop

; Get saved registers from stack
    pop rdi
    pop rcx
    pop rbx

    ret
createGaussianKernelAsm endp
;//////////////////////////////////


;//////////////////////////////////
; Description:
    ; Separate procedure performing only the 1st stage (horizontall) of the Gaussian blur. 
    ; Used in multithreading.
; Input: 
    ; rcx: bitmapData (pointing to bitmap data)
    ; rdx: array args [width, endHeight, stride, kernelSize, startHeight]
    ; r8: *tempData
    ; r9; *kernel
;///////////////  
;Registers used:
    ; rax   tempVal, selectedIndex
    ; rbx   tempVal, pixelIndex 
    ; rcx   y = startHeight
    ; rdx   x 
    ; r8    width   
    ; r9    height = endHeight
    ; r10   stride
    ; r11   kernelSize
    ; r12   pixelIndex
    ; r13   i (kernel index)
    ; r14   offset = kernelSize / 2; // we consider the middle index as 0 e.g. [-2, -1, 0, 1, 2]
    ; r15   *tempData
    ; rsi   *bitmapData
    ; rdi   *kernel
    ;
    ; xmm0 = xxxx rBlr gBlr bBlr  
    ; xmm1 = xxxx rImg gImg bImg 
    ; xmm3 = kerI kerI kerI kerI 
gaussBlurStage1Asm proc
; Save registers to stack
    push rbx
    push r12
    push r13
    push r14
    push r15
    push rsi
    push rdi
    sub rsp, 8              ; 16 byte alignment

; Prepare parameters  
    mov  rsi, rcx                       ; rsi = *bitmapData
    mov  rdi, r9                        ; rdi = *kernel
    mov  r15, r8                        ; r15 = *tempData

    xor  r8, r8   
    xor  r9, r9 
    xor  r10, r10 
    xor  r11, r11

    mov  r8d, dword ptr [rdx]            ; r8  = width   
    mov  r9d, dword ptr [rdx + 4]        ; r9  = height = endHeight
    mov  r10d, dword ptr [rdx + 8]       ; r10 = stride
    mov  r11d, dword ptr [rdx + 12]      ; r11 = kernelSize

    mov  r14, r11                        ; kernelSize -> r14
    shr  r14, 1                          ; r14 = offset = kernelSize / 2 

; Y-loop: Goes through half the height
    xor  rcx, rcx                       ; rcx -> y = 0
    mov  ecx, dword ptr [rdx + 16]      ; rcx = startHeight
y_loop_Stage1:
    cmp  rcx, r9                 ; if (y >= height), end loop
    jge  end_y_loop_Stage1

; X-loop: Goes through half the width
    xor  rdx, rdx                ; rdx -> x = 0
x_loop_Stage1:
    cmp  rdx, r8                 ; if (x >= width), end loop
    jge  end_x_loop_Stage1
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
i_loop_Stage1:
    cmp  r13, r11                ; if (i >= kernelSize), end I loop ->  (i < kernelSize)
    jge  end_i_loop_Stage1
    mov  rbx, r13                ; rbx = i;
    sub  rbx, r14                ; rbx = i - offset
    mov  rax, rbx                ; (i - offset) will reuse in a moment line 303
    add  rbx, rdx                ; rbx = selectedX = (i - offset) + x
    

    cmp  rbx, 0                  ; if (selectedX < 0) end -> (selectedX >= 0)
    jl   less_than_zero_Stage1         ; &&
    cmp  rbx, r8                 ; if (selectedX >= width) end -> (selectedX < width)
    jge  more_than_width_Stage1
    
    imul rax, 3                 ;((i - offset) * 3)
    mov  rbx, r12               ; rbx = pixelIndex
    add  rax, rbx               ; rax = selectedIndex = pixelIndex + ((i - offset) * 3)

    jmp continue_Stage1         ; skip next code fragment (represents other case)

;if out of border we want to mirror the edge 
;-2 -1 [ 0 1 ... n-1 n ] n+1 n+2     
; 1  0 [ 0 1 ... n-1 n ] n   n-1    

less_than_zero_Stage1:
    add  rbx, 1                 ; 1 + selectedX
    imul rbx, 3                 ; (1 + selectedX) * 3
    neg  rbx                    ; (1 + selectedX) * -3
    mov  rax, r12               ; rax = pixelIndex
    add  rax, rbx               ; selectedIndex += (1 + selectedX) * -3;

    jmp continue_Stage1         ; skip next code fragment (represents other case)

more_than_width_Stage1:
    add  rbx, 1                 ; 1 + selectedX
    mov  rax, r8                ; rax = width
    sub  rax, rbx               ; rax = width - 1 - selectedX = width - (1 + selectedX)
    imul rax, 3                 ; rax = (width - 1 - selectedX) * 3
    add  rax, r12               ; rax = selectedIndex += (width - 1 - selectedX) * 3;

continue_Stage1: 
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
    jmp i_loop_Stage1              ; back to I loop

end_i_loop_Stage1:
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
    jmp x_loop_Stage1              ; back to X loop

end_x_loop_Stage1:
    inc rcx                         ; y++
    jmp y_loop_Stage1               ; back to Y loop

end_y_loop_Stage1:
; Get saved registers from stack
    add rsp, 8              ; Restores the stack
    pop rdi
    pop rsi
    pop r15
    pop r14
    pop r13
    pop r12
    pop rbx

    ret
gaussBlurStage1Asm endp
;//////////////////////////////////


;//////////////////////////////////
; Description:
    ; Separate procedure performing only the 2nd stage (verticall) of the Gaussian blur. 
    ; Used in multithreading.
; Input: 
    ; rcx: bitmapData (pointing to bitmap data)
    ; rdx: array args [width, height, stride, kernelSize, startHeight, endHeight]
    ; r8: *tempData
    ; r9; *kernel
;///////////////  
;Registers used:
    ; rax   tempVal, selectedIndex
    ; rbx   tempVal, pixelIndex 
    ; rcx   y = startHeight
    ; rdx   x 
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
    ; xmm0 = xxxx rBlr gBlr bBlr  
    ; xmm1 = xxxx rImg gImg bImg 
    ; xmm3 = kerI kerI kerI kerI 
    ;
    ; [rsp]  endHeight
gaussBlurStage2Asm proc
; Save callee-saved registers to stack
    push rbx
    push r12
    push r13
    push r14
    push r15
    push rsi
    push rdi

; Init const on stack
; Decrease the stack pointer by 8 bytes. 4 to allocate space for an int 4 to align to 16 byte 
    sub rsp, 8                                  


; Assign the value endHeight to the variable on the stack
    xor rax, rax
    mov eax, dword ptr[rdx + 20]  
    mov dword ptr [rsp], eax    

; Prepare parameters  
    mov  rsi, rcx                       ; rsi = *bitmapData
    mov  rdi, r9                        ; rdi = *kernel
    mov  r15, r8                        ; r15 = *tempData

    xor  r8, r8   
    xor  r9, r9 
    xor  r10, r10 
    xor  r11, r11

    mov  r8d, dword ptr [rdx]            ; r8  = width   
    mov  r9d, dword ptr [rdx + 4]        ; r9  = height 
    mov  r10d, dword ptr [rdx + 8]       ; r10 = stride
    mov  r11d, dword ptr [rdx + 12]      ; r11 = kernelSize

    mov  r14, r11                        ; kernelSize -> r14
    shr  r14, 1                          ; r14 = offset = kernelSize / 2 

; Y-loop: Goes through half the height
    xor  rcx, rcx                       ; rcx -> y = 0
    mov  ecx, dword ptr [rdx + 16]      ; rcx = startHeight
y_loop_Stage2:
    mov eax, dword ptr [rsp]        ; eax = endHeight

    cmp  ecx, eax                   ; if (y >= endHeight), end loop
    jge  end_y_loop_Stage2

; X-loop: Goes through half the width
    xor  rdx, rdx               ; rdx -> x = 0
x_loop_Stage2:
    cmp  rdx, r8                ; if (x >= width), end loop
    jge  end_x_loop_Stage2
    ; TODO may be mul instead of imul?
    ; Calculate pixel index (pixelIndex = y * stride + x * 3)
    mov   r12, rcx              ; r12 = y
    imul  r12, r10              ; r12 = y * stride
    imul  rbx, rdx, 3           ; rbx = x * 3
    add   r12, rbx              ; r12 = pixelIndex = y * stride + x * 3    

    ; I-loop: goes through kernel
    ; xmm0 = xxxx rBlr gBlr bBlr  
    xorps xmm0, xmm0            ; blurredPixels (only 3 so 1 will be ignorred)
    xor   r13, r13              ; i

i_loop_Stage2:
    cmp  r13, r11                ; if (i >= kernelSize), end I loop ->  (i < kernelSize)
    jge  end_i_loop_Stage2
    mov  rbx, r13                ; rbx = i;
    sub  rbx, r14                ; rbx = i - offset
    mov  rax, rbx                ; (i - offset) will reuse in a moment
    add  rbx, rcx                ; rbx = (i - offset) + y
    

    cmp  rbx, 0                  ; if (selectedY < 0) end -> (selectedY >= 0)
    jl   less_than_zero_Stage2      ; &&
    cmp  rbx, r9                 ; if (selectedY >= height) end -> (selectedY < height)
    jge  more_than_height_Stage2
    
    imul rax, r10               ; ((i - offset) * stride)
    mov  rbx, r12               ; rbx = pixelIndex
    add  rax, rbx               ; rbx = selectedIndex = pixelIndex + ((i - offset) * stride)

    jmp continue_Stage2         ; skip next code fragment (represents other case)
less_than_zero_Stage2:
    add  rbx, 1                 ; 1 + selectedY
    imul rbx, r10               ; (1 + selectedY) * stride
    neg  rbx                    ; (1 + selectedY) * -stride
    mov  rax, r12               ; rax = pixelIndex
    add  rax, rbx               ; selectedIndex += (1 + selectedX) * -stride;

    jmp continue_Stage2         ; skip next code fragment (represents other case)

more_than_height_Stage2:

    add  rbx, 1                 ; 1 + selectedY
    mov  rax, r9                ; rax = height
    sub  rax, rbx               ; rax = height - 1 - selectedY = height - (1 + selectedy)
    imul rax, r10               ; rax = (height - 1 - selectedY) * stride
    add  rax, r12               ; rax = selectedIndex += (width - 1 - selectedY) * stride;

continue_Stage2:  
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

i_skip_Stage2:
    inc r13                        ; i++
    jmp i_loop_Stage2              ; back to I loop

end_i_loop_Stage2:
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
    jmp x_loop_Stage2              ; back to X loop

end_x_loop_Stage2:
    inc rcx                         ; y++
    jmp y_loop_Stage2               ; back to Y loop

end_y_loop_Stage2:

    add rsp, 8               ; Free up the space on the stack by restoring rsp

; Get saved registers from stack
    pop rdi
    pop rsi
    pop r15
    pop r14
    pop r13
    pop r12
    pop rbx

    ret
gaussBlurStage2Asm endp
;//////////////////////////////////


;//////////////////////////////////
; proc for testing if asm works
MyProc1 proc
; add RCX, RDX
; mov RAX, RCX
ret
MyProc1 endp
;//////////////////////////////////


;//////////////////////////////////
; Description:
    ; A Gaussian blur procedure using separable convolutions applies the blur
    ; in two stages first horizontally across the image, then vertically.
    ; Convolving each row of pixels with the given kernel horizontally
    ; and each column of the intermediate result vertically.
; Input: 
    ; rcx: bitmapData (pointing to bitmap data)
    ; rdx: array args [width, height, stride, kernelSize]
    ; r8: *tempData
    ; r9; *kernel
;Registers used:
    ; rax   tempVal, selectedIndex
    ; rbx   tempVal, pixelIndex 
    ; rcx   y 
    ; rdx   x 
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
    ; xmm0 = xxxx rBlr gBlr bBlr  
    ; xmm1 = xxxx rImg gImg bImg 
    ; xmm3 = kerI kerI kerI kerI 
gaussBlurAsm proc
; Save callee-saved registers to stack
    push rbx
    push r12
    push r13
    push r14
    push r15
    sub rsp, 8              ; 16 byte alignment

; Prepare parameters  
    mov  rsi, rcx                       ; rsi = *bitmapData
    mov  rdi, r9                        ; rdi = *kernel
    mov  r15, r8                        ; r15 = *tempData

    xor  r8, r8   
    xor  r9, r9 
    xor  r10, r10 
    xor  r11, r11

    mov  r8d, dword ptr [rdx]            ; r8  = width   
    mov  r9d, dword ptr [rdx + 4]        ; r9  = height
    mov  r10d, dword ptr [rdx + 8]       ; r10 = stride
    mov  r11d, dword ptr [rdx + 12]      ; r11 = kernelSize

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
i_loop_2nd: 
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
; Get saved registers from stack
    add rsp, 8              ; Restores the stack
    pop r15
    pop r14
    pop r13
    pop r12
    pop rbx

    ret
gaussBlurAsm endp
;//////////////////////////////////
end