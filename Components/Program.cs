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

            BitwiseNotGate bitNot = new BitwiseNotGate(2);
            if (!bitNot.TestGate())
                Console.WriteLine("bitNot bug");

            BitwiseAndGate bitAnd = new BitwiseAndGate(2);
            if (!bitAnd.TestGate())
                Console.WriteLine("bitAnd bug");

            BitwiseOrGate bitOr = new BitwiseOrGate(2);
            if (!bitOr.TestGate())
                Console.WriteLine("bitOr bug");

            BitwiseMux bitMux = new BitwiseMux(2);
            if (!bitMux.TestGate())
                Console.WriteLine("bitMux bug");

            BitwiseDemux bitDemux = new BitwiseDemux(2);
            if (!bitDemux.TestGate())
                Console.WriteLine("bitDemux bug");
            
            BitwiseMultiwayMux bitMultiMux = new BitwiseMultiwayMux(3,2);
            if(!bitMultiMux.TestGate())
                Console.WriteLine("bitMultiMux bug");
            
            BitwiseMultiwayDemux bitMultiDemux = new BitwiseMultiwayDemux(3,2);
            if(!bitMultiDemux.TestGate())
                Console.WriteLine("bitMultiDemux bug");

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

            if (bitMux.TestGate())
                Console.WriteLine("bitMux corr bug");
            
            if (bitDemux.TestGate())
                Console.WriteLine("bitDemux corr bug");
            
            if (bitMultiMux.TestGate())
                Console.WriteLine("bitMultiMux corr bug");
            
            if (bitMultiDemux.TestGate())
                Console.WriteLine("bitMultiDemux corr bug");

            Console.WriteLine("done");
            //Console.ReadLine();
        }
    }
}