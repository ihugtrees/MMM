using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class Program
    {
        static void Main(string[] args)
        {
            //This is an example of a testing code that you should run for all the gates that you create

            //Create and gate
            AndGate and = new AndGate();
            if (!and.TestGate())
                Console.WriteLine("and bug");
//
//            MultiBitAdder multiBitAdder = new MultiBitAdder(4);
//            if (!multiBitAdder.TestGate())
//                Console.WriteLine("multiBitAdder bug");
//
//            SingleBitRegister singleBitRegister = new SingleBitRegister();
//            if (!singleBitRegister.TestGate())
//                Console.WriteLine("singleBitRegister bug");
//
//            MultiBitRegister multiBitRegister = new MultiBitRegister(3);
//            if (!multiBitRegister.TestGate())
//                Console.WriteLine("multiBitRegister bug");

            Memory memory = new Memory(2,2);
            if (!memory.TestGate())
                Console.WriteLine("memory bug");

            //Now we ruin the nand gates that are used in all other gates. The gate should not work properly after this.
            NAndGate.Corrupt = true;
            if (and.TestGate())
                Console.WriteLine("and corr bug");

//            if (multiBitAdder.TestGate())
//                Console.WriteLine("multiBitAdder corr bug");
//
//            if (singleBitRegister.TestGate())
//                Console.WriteLine("singleBitRegister corr bug");
//
//            if (multiBitRegister.TestGate())
//                Console.WriteLine("multiBitRegister corr bug");

            if (memory.TestGate())
                Console.WriteLine("memory corr bug");

            Console.WriteLine("done");
        }
    }
}