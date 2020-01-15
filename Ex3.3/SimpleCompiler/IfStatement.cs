using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class IfStatement : StatetmentBase
    {
        public Expression Term { get; private set; }
        public List<StatetmentBase> DoIfTrue { get; private set; }
        public List<StatetmentBase> DoIfFalse { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            DoIfTrue = new List<StatetmentBase>();
            DoIfFalse = new List<StatetmentBase>();

            //We check that the first token is "if"
            Token tFunc = sTokens.Pop();
            if (!(tFunc is Statement) || ((Statement) tFunc).Name != "if")
                throw new SyntaxErrorException("Expected if received: " + tFunc, tFunc);

            //After the while there should be opening paranthesis for the arguments
            Token t = sTokens.Pop(); // (
            if (!(t is Parentheses) || ((Parentheses) t).Name != '(')
                throw new SyntaxErrorException("Expected ( received: " + t, t);

            Term = Expression.Create(sTokens);
            Term.Parse(sTokens);

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
                DoIfTrue.Add(s);
            }

            sTokens.Pop(); // }
            if (sTokens.Peek().ToString().Equals("else"))
            {
                //We check that the first token is "else"
                tFunc = sTokens.Pop();
                if (!(tFunc is Statement) || ((Statement) tFunc).Name != "else")
                    throw new SyntaxErrorException("Expected else received: " + tFunc, tFunc);

                t = sTokens.Pop(); // {
                if (!(t is Parentheses) || ((Parentheses) t).Name != '{')
                    throw new SyntaxErrorException("Expected { received: " + t, t);

                while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
                {
                    //We create the correct Statement type (if, while, return, let) based on the top token in the stack
                    StatetmentBase s = StatetmentBase.Create(sTokens.Peek());
                    //And call the Parse method of the statement to parse the different parts of the statement 
                    s.Parse(sTokens);
                    DoIfFalse.Add(s);
                }

                t = sTokens.Pop(); // }
                if (!(t is Parentheses) || ((Parentheses) t).Name != '}')
                    throw new SyntaxErrorException("Expected } received: " + t, t);
            }
        }

        public override string ToString()
        {
            string sIf = "if(" + Term + "){\n";
            foreach (StatetmentBase s in DoIfTrue)
                sIf += "\t\t\t" + s + "\n";
            sIf += "\t\t}";
            if (DoIfFalse.Count > 0)
            {
                sIf += "else{";
                foreach (StatetmentBase s in DoIfFalse)
                    sIf += "\t\t\t" + s + "\n";
                sIf += "\t\t}";
            }

            return sIf;
        }
    }
}