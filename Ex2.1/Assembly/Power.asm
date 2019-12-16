(START)
@2
D=M
@ZERO
D;JEQ
@1
D=M ;// D=N
@0
M=D ;//SUM=N
@2
D=M-1 ;//D=M-1
@I
M=D ;//I=M-1
(POW_LOOP)

(INNER_LOOP)


(ZERO)
@0
M=1
(END)
@END
0;JMP