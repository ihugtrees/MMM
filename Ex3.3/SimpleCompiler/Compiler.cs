using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class Compiler
    {
        public Compiler()
        {
        }

        public List<VarDeclaration> ParseVarDeclarations(List<string> lVarLines)
        {
            List<VarDeclaration> lVars = new List<VarDeclaration>();
            for (int i = 0; i < lVarLines.Count; i++)
            {
                List<Token> lTokens = Tokenize(lVarLines[i], i);
                TokensStack stack = new TokensStack(lTokens);
                VarDeclaration var = new VarDeclaration();
                var.Parse(stack);
                lVars.Add(var);
            }

            return lVars;
        }


        public List<LetStatement> ParseAssignments(List<string> lLines)
        {
            List<LetStatement> lParsed = new List<LetStatement>();
            List<Token> lTokens = Tokenize(lLines);
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            while (sTokens.Count > 0)
            {
                LetStatement ls = new LetStatement();
                ls.Parse(sTokens);
                lParsed.Add(ls);
            }

            return lParsed;
        }


        public List<string> GenerateCode(LetStatement aSimple, Dictionary<string, int> dSymbolTable)
        {
            List<string> lAssembly = new List<string>();
            if (!dSymbolTable.ContainsKey(aSimple.Variable))
            {
                throw new Exception("No such declaration in symbol table");
            }

            //add here code for computing a single let statement containing only a simple expression
            if (aSimple.Value.GetType() == typeof(NumericExpression))
            {
                lAssembly.Add("@" + aSimple.Value);
                lAssembly.Add("D=A");
                lAssembly.AddRange(resultToLocal(aSimple,dSymbolTable));
            }
            else if (aSimple.Value.GetType() == typeof(BinaryOperationExpression))
            {
                BinaryOperationExpression exp = (BinaryOperationExpression) aSimple.Value;
                lAssembly.AddRange(CopyToOperand1(exp.Operand1));
                lAssembly.AddRange(CopyToOperand2(exp.Operand2));
                lAssembly.Add("@OPERAND1");
                lAssembly.Add("D=M");
                lAssembly.Add("@OPERAND2");
                lAssembly.Add("D=D"+exp.Operator+"M");
                lAssembly.AddRange(resultToLocal(aSimple,dSymbolTable));
            }

            return lAssembly;
        }

        private List<string> resultToLocal(LetStatement aSimple, Dictionary<string, int> dSymbolTable)
        {
            List<string> assembly = new List<string>();
            
            assembly.Add("@RESULT");
            assembly.Add("M=D");
            assembly.Add("@" + aSimple.Variable);
            assembly.Add("M=D");
            assembly.Add("@LCL");
            assembly.Add("D=M");
            assembly.Add("@" + dSymbolTable[aSimple.Variable]);
            assembly.Add("D=D+A");
            assembly.Add("@INDEX");
            assembly.Add("M=D");
            assembly.Add("@RESULT");
            assembly.Add("D=M");
            assembly.Add("@INDEX");
            assembly.Add("A=M");
            assembly.Add("M=D");

            return assembly;
        }

        private List<string> CopyToOperand1(Expression operand1)
        {
            List<string> assembly = new List<string>();
            if (operand1.GetType() == typeof(NumericExpression))
            {
                assembly.Add("@" + operand1);
                assembly.Add("D=A");
                assembly.Add("@OPERAND1");
                assembly.Add("M=D");
            }
            else
            {
                assembly.Add("@" + operand1);
                assembly.Add("D=M");
                assembly.Add("@OPERAND1");
                assembly.Add("M=D");
            }

            return assembly;
        }

        private List<string> CopyToOperand2(Expression operand2)
        {
            List<string> assembly = new List<string>();
            if (operand2.GetType() == typeof(NumericExpression))
            {
                assembly.Add("@" + operand2);
                assembly.Add("D=A");
                assembly.Add("@OPERAND2");
                assembly.Add("M=D");
            }
            else
            {
                assembly.Add("@" + operand2);
                assembly.Add("D=M");
                assembly.Add("@OPERAND2");
                assembly.Add("M=D");
            }

            return assembly;
        }


        public Dictionary<string, int> ComputeSymbolTable(List<VarDeclaration> lDeclerations)
        {
            Dictionary<string, int> dTable = new Dictionary<string, int>();
            //add here code to comptue a symbol table for the given var declarations
            //real vars should come before (lower indexes) than artificial vars (starting with _), and their indexes must be by order of appearance.
            //for example, given the declarations:
            //var int x;
            //var int _1;
            //var int y;
            //the resulting table should bae x=0,y=1,_1=2
            //throw an exception if a var with the same name is defined more than once

            int counter = 0;
            foreach (var var in lDeclerations)
            {
                if (var.Name[0] != '_')
                {
                    dTable.Add(var.Name, counter);
                    counter++;
                }
            }

            foreach (var var in lDeclerations)
            {
                if (var.Name[0] == '_')
                {
                    dTable.Add(var.Name, counter);
                    counter++;
                }
            }

            return dTable;
        }


        public List<string> GenerateCode(List<LetStatement> lSimpleAssignments, List<VarDeclaration> lVars)
        {
            List<string> lAssembly = new List<string>();
            Dictionary<string, int> dSymbolTable = ComputeSymbolTable(lVars);
            foreach (LetStatement aSimple in lSimpleAssignments)
                lAssembly.AddRange(GenerateCode(aSimple, dSymbolTable));
            return lAssembly;
        }

        public List<LetStatement> SimplifyExpressions(LetStatement s, List<VarDeclaration> lVars)
        {
            List<LetStatement> statements = new List<LetStatement>();
            //add here code to simply expressins in a statement. 
            //add var declarations for artificial variables.

            int counter = 1;
            if (s.Value.GetType() == typeof(BinaryOperationExpression))
            {
                BinaryOperationExpression ss = (BinaryOperationExpression) s.Value;
                if (ss.Operand1.GetType() == typeof(BinaryOperationExpression))
                {
                    LetStatement l = new LetStatement();
                    l.Variable = "_"+counter;
                    counter++;
                }
                
            }
            
            return null;
        }

        public List<LetStatement> SimplifyExpressions(List<LetStatement> ls, List<VarDeclaration> lVars)
        {
            List<LetStatement> lSimplified = new List<LetStatement>();
            foreach (LetStatement s in ls)
                lSimplified.AddRange(SimplifyExpressions(s, lVars));
            return lSimplified;
        }


        public LetStatement ParseStatement(List<Token> lTokens)
        {
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            LetStatement s = new LetStatement();
            s.Parse(sTokens);
            return s;
        }


        public List<Token> Tokenize(string sLine, int iLine)
        {
            List<Token> lTokens = new List<Token>();

            if (sLine.Contains("//")) // handle comments and empty lines
            {
                int indexOfComment = sLine.IndexOf("//");
                if (indexOfComment == 0)
                {
                    return lTokens;
                }

                sLine = sLine.Substring(0, indexOfComment);
            }

            var myList = new List<char>();
            myList.AddRange(Token.Parentheses);
            myList.AddRange(Token.Separators);
            myList.AddRange(Token.Operators);
            myList.Add(' ');
            myList.Add('\t');
            char[] delimiters = myList.ToArray();

            List<string> tokens = Split(sLine, delimiters);
            int posIndex = 0;

            for (var j = 0; j < tokens.Count; j++)
            {
                var token = tokens[j];

                if (token.Equals("\t") || token.Equals(" "))
                {
                    posIndex += 1;
                    continue;
                }

                if (Token.Statements.Contains(token))
                {
                    lTokens.Add(new Statement(token, iLine, posIndex));
                }
                else if (Token.Constants.Contains(token))
                {
                    lTokens.Add(new Constant(token, iLine, posIndex));
                }
                else if (Token.VarTypes.Contains(token))
                {
                    lTokens.Add(new VarType(token, iLine, posIndex));
                }
                else if (Token.Operators.Contains(token.First()))
                {
                    lTokens.Add(new Operator(token[0], iLine, posIndex));
                }
                else if (Token.Parentheses.Contains(token.First()))
                {
                    lTokens.Add(new Parentheses(token[0], iLine, posIndex));
                }
                else if (Token.Separators.Contains(token.First()))
                {
                    lTokens.Add(new Separator(token[0], iLine, posIndex));
                }
                else if (Int32.TryParse(token, out var x))
                {
                    lTokens.Add(new Number(token, iLine, posIndex));
                }
                else
                {
                    char first = token[0];
                    if (!(Char.IsLetter(first)))
                    {
                        Token problematic = new Identifier(token, iLine, posIndex);
                        throw new SyntaxErrorException("identifier can't start with number", problematic);
                    }

                    lTokens.Add(new Identifier(token, iLine, posIndex));
                }

                posIndex += token.Length;
            }

            return lTokens;
        }


        public List<Token> Tokenize(List<string> lCodeLines)
        {
            List<Token> lTokens = new List<Token>();
            for (int i = 0; i < lCodeLines.Count; i++)
            {
                string sLine = lCodeLines[i];
                List<Token> lLineTokens = Tokenize(sLine, i);
                lTokens.AddRange(lLineTokens);
            }

            return lTokens;
        }

        private List<string> Split(string s, char[] aDelimiters)
        {
            List<string> lTokens = new List<string>();
            while (s.Length > 0)
            {
                string sToken = "";
                int i = 0;
                for (i = 0; i < s.Length; i++)
                {
                    if (aDelimiters.Contains(s[i]))
                    {
                        if (sToken.Length > 0)
                            lTokens.Add(sToken);
                        lTokens.Add(s[i] + "");
                        break;
                    }
                    else
                        sToken += s[i];
                }

                if (i == s.Length)
                {
                    lTokens.Add(sToken);
                    s = "";
                }
                else
                    s = s.Substring(i + 1);
            }

            return lTokens;
        }
    }
}