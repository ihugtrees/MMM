﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public class Assembler
    {
        private const int WORD_SIZE = 16;

        private Dictionary<string, int[]>
            m_dControl,
            m_dJmp,
            m_dDest; //these dictionaries map command mnemonics to machine code - they are initialized at the bottom of the class

        //more data structures here (symbol map, ...)

        public Assembler()
        {
            InitCommandDictionaries();
        }

        //this method is called from the outside to run the assembler translation
        public void TranslateAssemblyFile(string sInputAssemblyFile, string sOutputMachineCodeFile)
        {
            //read the raw input, including comments, errors, ...
            StreamReader sr = new StreamReader(sInputAssemblyFile);
            List<string> lLines = new List<string>();
            while (!sr.EndOfStream)
            {
                lLines.Add(sr.ReadLine());
            }

            sr.Close();
            //translate to machine code
            List<string> lTranslated = TranslateAssemblyFile(lLines);
            //write the output to the machine code file
            StreamWriter sw = new StreamWriter(sOutputMachineCodeFile);
            foreach (string sLine in lTranslated)
                sw.WriteLine(sLine);
            sw.Close();
        }

        //translate assembly into machine code
        private List<string> TranslateAssemblyFile(List<string> lLines)
        {
            //implementation order:
            //first, implement "TranslateAssemblyToMachineCode", and check if the examples "Add", "MaxL" are translated correctly.
            //next, implement "CreateSymbolTable", and modify the method "TranslateAssemblyToMachineCode" so it will support symbols (translating symbols to numbers). check this on the examples that don't contain macros
            //the last thing you need to do, is to implement "ExpendMacro", and test it on the example: "SquareMacro.asm".
            //init data structures here 

            //expand the macros
            List<string> lAfterMacroExpansion = ExpendMacros(lLines);

            //first pass - create symbol table and remove lable lines
            CreateSymbolTable(lAfterMacroExpansion);

            //second pass - replace symbols with numbers, and translate to machine code
            List<string> lAfterTranslation = TranslateAssemblyToMachineCode(lAfterMacroExpansion);
            return lAfterTranslation;
        }


        //first pass - replace all macros with real assembly
        private List<string> ExpendMacros(List<string> lLines)
        {
            //You do not need to change this function, you only need to implement the "ExapndMacro" method (that gets a single line == string)
            List<string> lAfterExpansion = new List<string>();
            for (int i = 0; i < lLines.Count; i++)
            {
                //remove all redudant characters
                string sLine = CleanWhiteSpacesAndComments(lLines[i]);
                if (sLine == "")
                    continue;
                //if the line contains a macro, expand it, otherwise the line remains the same
                List<string> lExpanded = ExapndMacro(sLine);
                //we may get multiple lines from a macro expansion
                foreach (string sExpanded in lExpanded)
                {
                    lAfterExpansion.Add(sExpanded);
                }
            }

            return lAfterExpansion;
        }

        //expand a single macro line
        private List<string> ExapndMacro(string sLine)
        {
            List<string> lExpanded = new List<string>();

            if (IsCCommand(sLine))
            {
                string sDest, sCompute, sJmp;
                GetCommandParts(sLine, out sDest, out sCompute, out sJmp);
                //your code here - check for indirect addessing and for jmp shortcuts
                //read the word file to see all the macros you need to support
            }

            if (lExpanded.Count == 0)
                lExpanded.Add(sLine);
            return lExpanded;
        }

        //second pass - record all symbols - labels and variables
        private void CreateSymbolTable(List<string> lLines)
        {
            string sLine = "";
            for (int i = 0; i < lLines.Count; i++)
            {
                sLine = lLines[i];
                if (IsLabelLine(sLine))
                {
                    //record label in symbol table
                    //do not add the label line to the result
                }
                else if (IsACommand(sLine))
                {
                    //may contain a variable - if so, record it to the symbol table (if it doesn't exist there yet...)
                }
                else if (IsCCommand(sLine))
                {
                    //do nothing here
                }
                else
                    throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
            }
        }

        //third pass - translate lines into machine code, replacing symbols with numbers
        private List<string> TranslateAssemblyToMachineCode(List<string> lLines)
        {
            string sLine = "";
            List<string> lAfterPass = new List<string>();
            for (int i = 0; i < lLines.Count; i++)
            {
                sLine = lLines[i];
                if (IsACommand(sLine))
                {
                    //translate an A command into a sequence of bits
                    string snumber = sLine.Substring(1);
                    int number = Int32.Parse(snumber);
                    lAfterPass.Add(ToBinary(number));
                }
                else if (IsCCommand(sLine))
                {
                    string sDest, sControl, sJmp;
                    GetCommandParts(sLine, out sDest, out sControl, out sJmp);
                    //translate an C command into a sequence of bits
                    //take a look at the dictionaries m_dControl, m_dJmp, and where they are initialized (InitCommandDictionaries), to understand how to you them here
                    int[] arrBinJmp, arrBinControl, arrBinDest;
                    string strBinJmp, strBinControl, strBinDest;

                    m_dJmp.TryGetValue(sJmp, out arrBinJmp);
                    strBinJmp = ToString(arrBinJmp);

                    m_dControl.TryGetValue(sControl, out arrBinControl);
                    strBinControl = ToString(arrBinControl);

                    m_dDest.TryGetValue(sDest, out arrBinDest);
                    strBinDest = ToString(arrBinDest);

                    string final = strBinControl + strBinDest + strBinJmp;
                    lAfterPass.Add(final);
                }
                else
                    throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
            }

            return lAfterPass;
        }

        //helper functions for translating numbers or bits into strings
        private string ToString(int[] aBits)
        {
            string sBinary = "";
            for (int i = 0; i < aBits.Length; i++)
                sBinary += aBits[i];
            return sBinary;
        }

        private string ToBinary(int x)
        {
            string sBinary = "";
            for (int i = 0; i < WORD_SIZE; i++)
            {
                sBinary = (x % 2) + sBinary;
                x = x / 2;
            }

            return sBinary;
        }


        //helper function for splitting the various fields of a C command
        private void GetCommandParts(string sLine, out string sDest, out string sControl, out string sJmp)
        {
            if (sLine.Contains('='))
            {
                int idx = sLine.IndexOf('=');
                sDest = sLine.Substring(0, idx);
                sLine = sLine.Substring(idx + 1);
            }
            else
                sDest = "";

            if (sLine.Contains(';'))
            {
                int idx = sLine.IndexOf(';');
                sControl = sLine.Substring(0, idx);
                sJmp = sLine.Substring(idx + 1);
            }
            else
            {
                sControl = sLine;
                sJmp = "";
            }
        }

        private bool IsCCommand(string sLine)
        {
            return !IsLabelLine(sLine) && sLine[0] != '@';
        }

        private bool IsACommand(string sLine)
        {
            return sLine[0] == '@';
        }

        private bool IsLabelLine(string sLine)
        {
            if (sLine.StartsWith("(") && sLine.EndsWith(")"))
                return true;
            return false;
        }

        private string CleanWhiteSpacesAndComments(string sDirty)
        {
            string sClean = "";
            for (int i = 0; i < sDirty.Length; i++)
            {
                char c = sDirty[i];
                if (c == '/' && i < sDirty.Length - 1 && sDirty[i + 1] == '/') // this is a comment
                    return sClean;
                if (c > ' ' && c <= '~') //ignore white spaces
                    sClean += c;
            }

            return sClean;
        }


        private void InitCommandDictionaries()
        {
            m_dControl = new Dictionary<string, int[]>();

            m_dControl["0"] = new int[] {0, 1, 0, 1, 0, 1, 0};
            m_dControl["1"] = new int[] {0, 1, 1, 1, 1, 1, 1};
            m_dControl["-1"] = new int[] {0, 1, 1, 1, 0, 1, 0};
            m_dControl["D"] = new int[] {0, 0, 0, 1, 1, 0, 0};
            m_dControl["A"] = new int[] {0, 1, 1, 0, 0, 0, 0};
            m_dControl["!D"] = new int[] {0, 0, 0, 1, 1, 0, 1};
            m_dControl["!A"] = new int[] {0, 1, 1, 0, 0, 0, 1};
            m_dControl["-D"] = new int[] {0, 0, 0, 1, 1, 1, 1};
            m_dControl["-A"] = new int[] {0, 1, 1, 0, 0, 1, 1};
            m_dControl["D+1"] = new int[] {0, 0, 1, 1, 1, 1, 1};
            m_dControl["A+1"] = new int[] {0, 1, 1, 0, 1, 1, 1};
            m_dControl["D-1"] = new int[] {0, 0, 0, 1, 1, 1, 0};
            m_dControl["A-1"] = new int[] {0, 1, 1, 0, 0, 1, 0};
            m_dControl["D+A"] = new int[] {0, 0, 0, 0, 0, 1, 0};
            m_dControl["D-A"] = new int[] {0, 0, 1, 0, 0, 1, 1};
            m_dControl["A-D"] = new int[] {0, 0, 0, 0, 1, 1, 1};
            m_dControl["D&A"] = new int[] {0, 0, 0, 0, 0, 0, 0};
            m_dControl["D|A"] = new int[] {0, 0, 1, 0, 1, 0, 1};

            m_dControl["M"] = new int[] {1, 1, 1, 0, 0, 0, 0};
            m_dControl["!M"] = new int[] {1, 1, 1, 0, 0, 0, 1};
            m_dControl["-M"] = new int[] {1, 1, 1, 0, 0, 1, 1};
            m_dControl["M+1"] = new int[] {1, 1, 1, 0, 1, 1, 1};
            m_dControl["M-1"] = new int[] {1, 1, 1, 0, 0, 1, 0};
            m_dControl["D+M"] = new int[] {1, 0, 0, 0, 0, 1, 0};
            m_dControl["D-M"] = new int[] {1, 0, 1, 0, 0, 1, 1};
            m_dControl["M-D"] = new int[] {1, 0, 0, 0, 1, 1, 1};
            m_dControl["D&M"] = new int[] {1, 0, 0, 0, 0, 0, 0};
            m_dControl["D|M"] = new int[] {1, 0, 1, 0, 1, 0, 1};


            m_dJmp = new Dictionary<string, int[]>();

            m_dJmp[""] = new int[] {0, 0, 0};
            m_dJmp["JGT"] = new int[] {0, 0, 1};
            m_dJmp["JEQ"] = new int[] {0, 1, 0};
            m_dJmp["JGE"] = new int[] {0, 1, 1};
            m_dJmp["JLT"] = new int[] {1, 0, 0};
            m_dJmp["JNE"] = new int[] {1, 0, 1};
            m_dJmp["JLE"] = new int[] {1, 1, 0};
            m_dJmp["JMP"] = new int[] {1, 1, 1};

            m_dDest = new Dictionary<string, int[]>();

            m_dDest[""] = new int[] {0, 0, 0};
            m_dDest["M"] = new int[] {0, 0, 1};
            m_dDest["D"] = new int[] {0, 1, 0};
            m_dDest["MD"] = new int[] {0, 1, 1};
            m_dDest["A"] = new int[] {1, 0, 0};
            m_dDest["AM"] = new int[] {1, 0, 1};
            m_dDest["AD"] = new int[] {1, 1, 0};
            m_dDest["AMD"] = new int[] {1, 1, 1};
        }
    }
}