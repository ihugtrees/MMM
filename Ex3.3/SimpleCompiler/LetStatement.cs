using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class LetStatement : StatetmentBase
    {
        public string Variable { get; set; }
        public Expression Value { get; set; }

        public override string ToString()
        {
            return "let " + Variable + " = " + Value + ";";
        }

        public override void Parse(TokensStack sTokens)
        {
            //We check that the first token is "let"
            Token tFunc = sTokens.Pop();
            if (!(tFunc is Statement) || ((Statement) tFunc).Name != "let")
                throw new SyntaxErrorException("Expected while received: " + tFunc, tFunc);

            Expression var = Expression.Create(sTokens); // identifier
            var.Parse(sTokens);
            Variable = var.ToString();
            
            Token t = sTokens.Pop(); // = 
            if (!(t is Operator) || ((Operator) t).Name != '=')
                throw new SyntaxErrorException("Expected = received: " + t, t);
            Value = Expression.Create(sTokens); // expression
            Value.Parse(sTokens);
            
            t = sTokens.Pop(); // ;
            if (!(t is Separator) || ((Separator) t).Name != ';')
                throw new SyntaxErrorException("Expected ; received: " + t, t);
        }
    }
}