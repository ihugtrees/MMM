﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembler a = new Assembler();
            //to run tests, call the "TranslateAssemblyFile" function like this:
            //string sourceFileLocation = the path to your source file
            //string destFileLocation = the path to your dest file
            //a.TranslateAssemblyFile(sourceFileLocation, destFileLocation);
            a.TranslateAssemblyFile(@"D:\BGU-PC\MMM\MMM\Ex2.3\Code\AssemblyExamples\test.asm"
                ,@"D:\BGU-PC\MMM\MMM\Ex2.3\Code\AssemblyExamples\AsmOutput\test.mc");
        }
    }
}