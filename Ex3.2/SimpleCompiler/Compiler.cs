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
        private Dictionary<string, int> m_dSymbolTable;
        private int m_cLocals;

        public Compiler()
        {
            m_dSymbolTable = new Dictionary<string, int>();
            m_cLocals = 0;
        }

        public List<string> Compile(string sInputFile)
        {
            List<string> lCodeLines = ReadFile(sInputFile);
            List<Token> lTokens = Tokenize(lCodeLines);
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            JackProgram program = Parse(sTokens);
            return null;
        }

        private JackProgram Parse(TokensStack sTokens)
        {
            JackProgram program = new JackProgram();
            program.Parse(sTokens);
            return program;
        }

        public List<string> Compile(List<string> lLines)
        {
            List<string> lCompiledCode = new List<string>();
            foreach (string sExpression in lLines)
            {
                List<string> lAssembly = Compile(sExpression);
                lCompiledCode.Add("// " + sExpression);
                lCompiledCode.AddRange(lAssembly);
            }

            return lCompiledCode;
        }


        public List<string> ReadFile(string sFileName)
        {
            StreamReader sr = new StreamReader(sFileName);
            List<string> lCodeLines = new List<string>();
            while (!sr.EndOfStream)
            {
                lCodeLines.Add(sr.ReadLine());
            }

            sr.Close();
            return lCodeLines;
        }


        public List<Token> Tokenize(List<string> lCodeLines)
        {
            {
                List<Token> lTokens = new List<Token>();
                //your code here
                var myList = new List<char>();
                myList.AddRange(Token.Parentheses);
                myList.AddRange(Token.Separators);
                myList.AddRange(Token.Operators);
                myList.Add(' ');
                myList.Add('\t');

                char[] delimiters = myList.ToArray();
                List<string> tokens = new List<string>();
                int lineIndex = 0;
                for (var i = 0; i < lCodeLines.Count; i++)
                {
                    int posIndex = 0;
                    var line = lCodeLines[i];
                    if (line.Contains("//")) // handle comments and empty lines
                    {
                        int indexOfComment = line.IndexOf("//");
                        if (indexOfComment == 0)
                        {
                            lineIndex++;
                            continue;
                        }

                        line = line.Substring(0, indexOfComment);
                    }

                    tokens = Split(line, delimiters);
                    for (var j = 0; j < tokens.Count; j++)
                    {
                        int number;
                        var token = tokens[j];

                        if (token.Equals("\t") || token.Equals(" "))
                        {
                            posIndex += 1;
                            continue;
                        }

                        if (Token.Statements.Contains(token))
                        {
                            lTokens.Add(new Statement(token, lineIndex, posIndex));
                        }
                        else if (Token.Constants.Contains(token))
                        {
                            lTokens.Add(new Constant(token, lineIndex, posIndex));
                        }
                        else if (Token.VarTypes.Contains(token))
                        {
                            lTokens.Add(new VarType(token, lineIndex, posIndex));
                        }
                        else if (Token.Operators.Contains(token.First()))
                        {
                            lTokens.Add(new Operator(token[0], lineIndex, posIndex));
                        }
                        else if (Token.Parentheses.Contains(token.First()))
                        {
                            lTokens.Add(new Parentheses(token[0], lineIndex, posIndex));
                        }
                        else if (Token.Separators.Contains(token.First()))
                        {
                            lTokens.Add(new Separator(token[0], lineIndex, posIndex));
                        }
                        else if (Int32.TryParse(token, out number))
                        {
                            lTokens.Add(new Number(token, lineIndex, posIndex));
                        }
                        else
                        {
                            char first = token[0];
                            if (!(Char.IsLetter(first)))
                            {
                                Token problematic = new Identifier(token, lineIndex, posIndex);
                                throw new SyntaxErrorException("identifier can't start with number", problematic);
                            }

                            lTokens.Add(new Identifier(token, lineIndex, posIndex));
                        }

                        posIndex += token.Length;
                    }

                    lineIndex++;
                }

                return lTokens;
            }
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