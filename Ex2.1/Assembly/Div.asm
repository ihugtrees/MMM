(START)
@1
D=M
@2
D=D-M
@1
M=D
@END
D;JLT
@0
M=M+1
@START
0;JMP
(END)
@END
0;JMP