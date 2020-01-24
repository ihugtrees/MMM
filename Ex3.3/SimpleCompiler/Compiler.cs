using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
                throw new SyntaxErrorException("No such declaration in symbol table",new Token());
            }

            //add here code for computing a single let statement containing only a simple expression
            if (aSimple.Value.GetType() == typeof(NumericExpression))
            {
                lAssembly.Add("@" + aSimple.Value);
                lAssembly.Add("D=A");
                lAssembly.Add("@RESULT");
                lAssembly.Add("M=D");
                lAssembly.AddRange(resultToLocal(aSimple, dSymbolTable));
            }
            else if (aSimple.Value.GetType() == typeof(VariableExpression))
            {
                lAssembly.Add("@LCL");
                lAssembly.Add("D=M");
                lAssembly.Add("@" + dSymbolTable[aSimple.Value.ToString()]);
                lAssembly.Add("D=D+A");
                lAssembly.Add("@ADDRESS");
                lAssembly.Add("M=D");
                lAssembly.Add("A=M");
                lAssembly.Add("D=M");
                lAssembly.Add("@RESULT");
                lAssembly.Add("M=D");
                lAssembly.AddRange(resultToLocal(aSimple, dSymbolTable));
            }
            else if (aSimple.Value.GetType() == typeof(BinaryOperationExpression))
            {
                BinaryOperationExpression exp = (BinaryOperationExpression) aSimple.Value;
                lAssembly.AddRange(CopyToOperand1(exp.Operand1, dSymbolTable));
                lAssembly.AddRange(CopyToOperand2(exp.Operand2, dSymbolTable));
                lAssembly.Add("@OPERAND1");
                lAssembly.Add("D=M");
                lAssembly.Add("@OPERAND2");
                lAssembly.Add("D=D" + exp.Operator + "M");
                lAssembly.Add("@RESULT");
                lAssembly.Add("M=D");
                lAssembly.AddRange(resultToLocal(aSimple, dSymbolTable));
            }

            return lAssembly;
        }

        private List<string> resultToLocal(LetStatement aSimple, Dictionary<string, int> dSymbolTable)
        {
            List<string> lAssembly = new List<string>();

            lAssembly.Add("@LCL");
            lAssembly.Add("D=M");
            lAssembly.Add("@" + dSymbolTable[aSimple.Variable]);
            lAssembly.Add("D=D+A");
            lAssembly.Add("@ADDRESS");
            lAssembly.Add("M=D");
            lAssembly.Add("@RESULT");
            lAssembly.Add("D=M");
            lAssembly.Add("@ADDRESS");
            lAssembly.Add("A=M");
            lAssembly.Add("M=D");

            return lAssembly;
        }

        private List<string> getLocalVarValue(string var, Dictionary<string, int> dSymbolTable)
        {
            List<string> assembly = new List<string>();

            assembly.Add("@LCL");
            assembly.Add("D=M");
            assembly.Add("@" + dSymbolTable[var]);
            assembly.Add("D=D+A");
            assembly.Add("@ADDRESS");
            assembly.Add("M=D");

            return assembly;
        }

        private List<string> CopyToOperand1(Expression operand1, Dictionary<string, int> dSymbolTable)
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
                assembly.AddRange(getLocalVarValue(operand1.ToString(), dSymbolTable));
                assembly.Add("@ADDRESS");
                assembly.Add("A=M");
                assembly.Add("D=M");
                assembly.Add("@OPERAND1");
                assembly.Add("M=D");
            }

            return assembly;
        }

        private List<string> CopyToOperand2(Expression operand2, Dictionary<string, int> dSymbolTable)
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
                assembly.AddRange(getLocalVarValue(operand2.ToString(), dSymbolTable));
                assembly.Add("@ADDRESS");
                assembly.Add("A=M");
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
                    if (dTable.ContainsKey(var.Name))
                    {
                        throw new SyntaxErrorException("variable already exists",new Token());
                    }
                    dTable.Add(var.Name, counter);
                    counter++;
                }
            }

            foreach (var var in lDeclerations)
            {
                if (var.Name[0] == '_')
                {
                    if (dTable.ContainsKey(var.Name))
                    {
                        throw new SyntaxErrorException("variable already exists",new Token());
                    }
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


        public static Int32 counter = 1;

        public List<LetStatement> SimplifyExpressions(LetStatement s, List<VarDeclaration> lVars)
        {
            List<LetStatement> statements = new List<LetStatement>();
            recursiveLet(s, lVars, statements);

            //add here code to simply expressins in a statement. 
            //add var declarations for artificial variables.

            // LetStatement left = new LetStatement();
            // LetStatement right = new LetStatement();
            // LetStatement combined = new LetStatement();
            // combined.Variable = s.Variable;
            //
            // if (s.Value.GetType() == typeof(BinaryOperationExpression))
            // {
            //     BinaryOperationExpression value = (BinaryOperationExpression) s.Value;
            //
            //     BinaryOperationExpression binExp = new BinaryOperationExpression();
            //     binExp.Operator = value.Operator;
            //     VariableExpression var1 = new VariableExpression();
            //     VariableExpression var2 = new VariableExpression();
            //
            //     if (value.Operand1.GetType() == typeof(BinaryOperationExpression))
            //     {
            //         string virtualVar = "_" + counter;
            //         lVars.Add(new VarDeclaration("int", virtualVar));
            //         left.Variable = virtualVar;
            //         left.Value = value.Operand1;
            //         counter++;
            //
            //         var1.Name = virtualVar;
            //         binExp.Operand1 = var1;
            //
            //         recursiveLet(left, lVars, statements, counter);
            //         //statements.Add(left);
            //     }
            //     else
            //     {
            //         binExp.Operand1 = value.Operand1;
            //     }
            //
            //     if (value.Operand2.GetType() == typeof(BinaryOperationExpression))
            //     {
            //         string virtualVar = "_" + counter;
            //         lVars.Add(new VarDeclaration("int", virtualVar));
            //         right.Variable = virtualVar;
            //         right.Value = value.Operand2;
            //         counter++;
            //         
            //         var2.Name = virtualVar;
            //         binExp.Operand2 = var2;
            //
            //         recursiveLet(right, lVars, statements, counter);
            //         //statements.Add(right);
            //     }
            //     else
            //     {
            //         binExp.Operand2 = value.Operand2;
            //     }
            //
            //     combined.Value = binExp;
            //     statements.Add(combined);
            // }
            // else
            // {
            //     statements.Add(s);
            // }

            return statements;
        }

        public void recursiveLet(LetStatement s, List<VarDeclaration> lVars, List<LetStatement> statements)
        {
            if (s.Value.GetType() == typeof(BinaryOperationExpression))
            {
                BinaryOperationExpression value = (BinaryOperationExpression) s.Value;

                LetStatement left = new LetStatement();
                LetStatement right = new LetStatement();
                LetStatement combined = new LetStatement();

                combined.Variable = s.Variable;
                BinaryOperationExpression binExp = new BinaryOperationExpression();
                binExp.Operator = value.Operator;
                VariableExpression var1 = new VariableExpression();
                VariableExpression var2 = new VariableExpression();

                if (value.Operand1.GetType() == typeof(BinaryOperationExpression))
                {
                    string virtualVar = "_" + counter;
                    lVars.Add(new VarDeclaration("int", virtualVar));
                    left.Variable = virtualVar;
                    left.Value = value.Operand1;
                    counter++;

                    recursiveLet(left, lVars, statements);

                    var1.Name = left.Variable;
                    binExp.Operand1 = var1;

                    //statements.Add(left);
                }
                else
                {
                    binExp.Operand1 = value.Operand1;
                }

                if (value.Operand2.GetType() == typeof(BinaryOperationExpression))
                {
                    string virtualVar = "_" + counter;
                    lVars.Add(new VarDeclaration("int", virtualVar));
                    right.Variable = virtualVar;
                    right.Value = value.Operand2;
                    counter++;

                    recursiveLet(right, lVars, statements);

                    var2.Name = right.Variable;
                    binExp.Operand2 = var2;

                    //statements.Add(right);
                }
                else
                {
                    binExp.Operand2 = value.Operand2;
                }

                combined.Value = binExp;
                statements.Add(combined);
            }
            else
            {
                statements.Add(s);
            }
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