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

            //Create MultiBitAndGate gate
            MultiBitAndGate multiBitAndGate = new MultiBitAndGate(3);
            if (!multiBitAndGate.TestGate())
                Console.WriteLine("MultiBitAndGate bug");

            MuxGate mux = new MuxGate();
            if (!mux.TestGate())
                Console.WriteLine("mux bug");

            Demux demux = new Demux();
            if (!demux.TestGate())
                Console.WriteLine("mux bug");
            
            BitwiseNotGate bitNot= new BitwiseNotGate(2);
            if (!bitNot.TestGate())
                Console.WriteLine("bitNot bug");
            
            BitwiseAndGate bitAnd= new BitwiseAndGate(2);
            if (!bitAnd.TestGate())
                Console.WriteLine("bitAnd bug");
            
            BitwiseOrGate bitOr= new BitwiseOrGate(2);
            if (!bitOr.TestGate())
                Console.WriteLine("bitOr bug");

            //Now we ruin the nand gates that are used in all other gates. The gate should not work properly after this.
            NAndGate.Corrupt = true;
            if (and.TestGate())
                Console.WriteLine("and corr bug");

            if (multiBitAndGate.TestGate())
                Console.WriteLine("MultiBitAndGate corr bug");

            if (mux.TestGate())
                Console.WriteLine("mux corr bug");

            if (demux.TestGate())
                Console.WriteLine("demux corr bug");
            
            if (bitAnd.TestGate())
                Console.WriteLine("bitAnd corr bug");
            
            if (bitOr.TestGate())
                Console.WriteLine("bitOr corr bug");

            Console.WriteLine("done");
            //Console.ReadLine();
        }
    }
}