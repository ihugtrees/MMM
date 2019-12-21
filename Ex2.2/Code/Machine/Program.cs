using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleComponents;

namespace Machine
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Machine16 machine = new Machine16(false, true);
            machine.Code.LoadFromFile(@"D:\BGU-PC\MMM\MMM\Ex2.2\Assemblyexamples\Copy1To0.hack");
            machine.Data[0] = 0;
            machine.Data[1] = 15;
            DateTime dtStart = DateTime.Now;
            machine.Reset();
            for (int i = 0; i < 10; i++)
            {
                machine.CPU.PrintState();
                Console.WriteLine();
                Clock.ClockDown();
                Clock.ClockUp();
            }

            Console.WriteLine("R0 = "+machine.Data[0]);
            Console.WriteLine("Done " + (DateTime.Now - dtStart).TotalSeconds);
            Console.ReadLine();
        }
    }
}