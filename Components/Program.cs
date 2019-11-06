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

//            //Create or gate
//            OrGate or = new OrGate();
//            if (!or.TestGate())
//                Console.WriteLine("or bug");
//
//            //Create xor gate
//            XorGate xor = new XorGate();
//            if (!xor.TestGate())
//                Console.WriteLine("xor bug");

            //Create MultiBitAndGate gate
            MultiBitAndGate multiBitAndGate = new MultiBitAndGate(3);
           // if (!multiBitAndGate.TestGate())
           //     Console.WriteLine("MultiBitAndGate bug");

            //Create MultiBitOrGate gate
            MultiBitOrGate multiBitOrGate = new MultiBitOrGate(3);
            if (!multiBitOrGate.TestGate())
                Console.WriteLine("MultiBitOrGate bug");

            //Now we ruin the nand gates that are used in all other gates. The gate should not work properly after this.
            NAndGate.Corrupt = true;
            if (and.TestGate())
                Console.WriteLine("bugbug");
            
            if (multiBitAndGate.TestGate())
                Console.WriteLine("MultiBitAndGate bug");

            if (multiBitOrGate.TestGate())
                Console.WriteLine("MultiBitOrGate bug");


            Console.WriteLine("done");
            //Console.ReadLine();
        }
    }
}