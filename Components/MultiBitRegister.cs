using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class represents an n bit register that can maintain an n bit number
    class MultiBitRegister : Gate
    {
        public WireSet Input { get; private set; }

        public WireSet Output { get; private set; }

        //A bit setting the register operation to read or write
        public Wire Load { get; private set; }

        //Word size - number of bits in the register
        public int Size { get; private set; }

        private SingleBitRegister[] BitRegisters;


        public MultiBitRegister(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);
            Load = new Wire();

            BitRegisters = new SingleBitRegister[Size];
            for (int i = 0; i < Size; i++)
            {
                BitRegisters[i] = new SingleBitRegister();
                BitRegisters[i].ConnectLoad(Load);
                BitRegisters[i].ConnectInput(Input[i]);
                Output[i].ConnectInput(BitRegisters[i].Output);
            }
        }

        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        public void ConnectLoad(Wire wLoad)
        {
            Load.ConnectInput(wLoad);
        }


        public override string ToString()
        {
            return Output.ToString();
        }


        public override bool TestGate()
        {
            for (int i = 1; i >= 0; i--)
            {
                Load.Value = i;
                for (int j = 0; j < Math.Pow(2, Size); j++)
                {
                    Input.SetValue(j);
                    int lastOut = Output.GetValue();
                    Clock.ClockDown();
                    Clock.ClockUp();


//                    Console.WriteLine("Load: " + Load);
//                    Console.WriteLine("Input: " + Input);
//                    Console.WriteLine("Output: " + Output);
//                    Console.WriteLine("lastOut: " + lastOut);
//                    Console.WriteLine();

                    if (Load.Value == 1 && Input.GetValue() != Output.GetValue())
                    {
                        return false;
                    }

                    if (Load.Value == 0 && Output.GetValue() != lastOut && Input.GetValue() != lastOut)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}