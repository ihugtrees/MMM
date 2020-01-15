using System;
using System.Collections.Generic;

namespace SimpleCompiler
{
    public class FunctionCallExpression : Expression
    {
        public string FunctionName { get; private set; }
        public List<Expression> Args { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            Args = new List<Expression>();
            FunctionName = sTokens.Pop().ToString(); // func name
            
            Token t = sTokens.Pop(); // (
            if (!(t is Parentheses) || ((Parentheses) t).Name != '(')
                throw new SyntaxErrorException("Expected ( received: " + t, t);
            
            //Now we extract the arguments from the stack until we see a closing parathesis
            while(sTokens.Count > 0 && !(sTokens.Peek().ToString().Equals(")")))//)
            {
                Expression exp = Create(sTokens);
                exp.Parse(sTokens);
                Args.Add(exp);
                //If there is a comma, then there is another argument
                if (sTokens.Count > 0 && sTokens.Peek() is Separator)//,
                    sTokens.Pop(); 
            }
            
            t = sTokens.Pop(); // )
            if (!(t is Parentheses) || ((Parentheses) t).Name != ')')
                throw new SyntaxErrorException("Expected ) received: " + t, t);
        }

        public override string ToString()
        {
            string sFunction = FunctionName + "(";
            for (int i = 0; i < Args.Count - 1; i++)
                sFunction += Args[i] + ",";
            if (Args.Count > 0)
                sFunction += Args[Args.Count - 1];
            sFunction += ")";
            return sFunction;
        }
    }
}