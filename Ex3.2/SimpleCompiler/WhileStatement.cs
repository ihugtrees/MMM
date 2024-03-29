﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class WhileStatement : StatetmentBase
    {
        public Expression Term { get; private set; }
        public List<StatetmentBase> Body { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            Body = new List<StatetmentBase>();

            //We check that the first token is "while"
            Token tFunc = sTokens.Pop();
            if (!(tFunc is Statement) || ((Statement) tFunc).Name != "while")
                throw new SyntaxErrorException("Expected while received: " + tFunc, tFunc);

            //After the while there should be opening paranthesis for the arguments
            Token t = sTokens.Pop(); //(
            if (!(t is Parentheses) || ((Parentheses) t).Name != '(')
                throw new SyntaxErrorException("Expected ( received: " + t, t);

            Expression expr = Expression.Create(sTokens);
            expr.Parse(sTokens);
            Term = expr;
            
            t = sTokens.Pop(); // )
            if (!(t is Parentheses) || ((Parentheses) t).Name != ')')
                throw new SyntaxErrorException("Expected ) received: " + t, t);
            t = sTokens.Pop(); // {
            if (!(t is Parentheses) || ((Parentheses) t).Name != '{')
                throw new SyntaxErrorException("Expected { received: " + t, t);

            while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
            {
                //We create the correct Statement type (if, while, return, let) based on the top token in the stack
                StatetmentBase s = StatetmentBase.Create(sTokens.Peek());
                //And call the Parse method of the statement to parse the different parts of the statement 
                s.Parse(sTokens);
                Body.Add(s);
            }

            t = sTokens.Pop(); // }
            if (!(t is Parentheses) || ((Parentheses) t).Name != '}')
                throw new SyntaxErrorException("Expected } received: " + t, t);
        }

        public override string ToString()
        {
            string sWhile = "while(" + Term + "){\n";
            foreach (StatetmentBase s in Body)
                sWhile += "\t\t\t" + s + "\n";
            sWhile += "\t\t}";
            return sWhile;
        }
    }
}