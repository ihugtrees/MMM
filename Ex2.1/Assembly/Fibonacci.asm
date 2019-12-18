(START)
@1
A=M
M=1 //ARR[0]=1
@ADDER
M=0 //A=0
@I
M=1 //I=1
D=M
@1
D=D+M //I+ARR
@INDX
M=D //INDX=ARR+I
(LOOP)
@ADDER
D=M //D=a
@INDX
A=M-1
D=D+M //a+arr[i-1]
@INDX
A=M
M=D //arr[i] = a+arr[i-1]
@ADDER
M=D-M //a=arr[i-1]
@2
D=M
@I
M=M+1
D=D-M //n-i
@INDX
M=M+1
@LOOP
D;JGT
(END)
@END
0;JMP