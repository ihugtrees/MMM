﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class BinaryOperationExpression : Expression
    {
        public string Operator { get;  set; }
        public Expression Operand1 { get;  set; }
        public Expression Operand2 { get;  set; }

        public override string ToString()
        {
            return "(" + Operand1 + " " + Operator + " " + Operand2 + ")";
        }

        public override void Parse(TokensStack sTokens)
        {
            sTokens.Pop(); // (
            Operand1 = Create(sTokens);
            Operand1.Parse(sTokens);
            Operator = sTokens.Pop().ToString();
            Operand2 = Create(sTokens);
            Operand2.Parse(sTokens);
            sTokens.Pop(); // )
        }
    }
}
