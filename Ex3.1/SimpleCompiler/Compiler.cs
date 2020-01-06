using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleCompiler
{
    class Compiler
    {
        public Compiler()
        {
        }

        //reads a file into a list of strings, each string represents one line of code
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


        //Computes the next token in the string s, from the begining of s until a delimiter has been reached. 
        //Returns the string without the token.
        private string Next(string s, char[] aDelimiters, out string sToken, out int cChars)
        {
            cChars = 1;
            sToken = s[0] + "";
            if (aDelimiters.Contains(s[0]))
                return s.Substring(1);
            int i = 0;
            for (i = 1; i < s.Length; i++)
            {
                if (aDelimiters.Contains(s[i]))
                    return s.Substring(i);
                else
                    sToken += s[i];
                cChars++;
            }

            return null;
        }

        //Splits a string into a list of tokens, separated by delimiters
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

        //This is the main method for the Tokenizing assignment. 
        //Takes a list of code lines, and returns a list of tokens.
        //For each token you must identify its type, and instantiate the correct subclass accordingly.
        //You need to identify the token position in the file (line, index within the line).
        //You also need to identify errors, in this assignement - illegal identifier names.
        public List<Token> Tokenize(List<string> lCodeLines)
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
}