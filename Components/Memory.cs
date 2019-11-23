using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a memory unit, containing k registers, each of size n bits.
    class Memory : SequentialGate
    {
        //The address size determines the number of registers
        public int AddressSize { get; private set; }

        //The word size determines the number of bits in each register
        public int WordSize { get; private set; }

        //Data input and output - a number with n bits
        public WireSet Input { get; private set; }

        public WireSet Output { get; private set; }

        //The address of the active register
        public WireSet Address { get; private set; }

        //A bit setting the memory operation to read or write
        public Wire Load { get; private set; }

        private BitwiseMultiwayMux mux;
        private BitwiseMultiwayDemux demux;
        private MultiBitRegister[] MultiBitRegisters;
        private WireSet wsLoad;
        private int numOfRegisters;

        public Memory(int iAddressSize, int iWordSize)
        {
            AddressSize = iAddressSize;
            WordSize = iWordSize;

            Input = new WireSet(WordSize);
            Output = new WireSet(WordSize);
            Address = new WireSet(AddressSize);
            Load = new Wire();
            wsLoad = new WireSet(1);

            mux = new BitwiseMultiwayMux(WordSize, AddressSize);
            demux = new BitwiseMultiwayDemux(1, AddressSize);

            numOfRegisters = (int) Math.Pow(2, AddressSize);
            MultiBitRegisters = new MultiBitRegister[numOfRegisters];

            for (int i = 0; i < numOfRegisters; i++)
            {
                MultiBitRegisters[i] = new MultiBitRegister(WordSize);
                MultiBitRegisters[i].ConnectInput(Input);
                MultiBitRegisters[i].ConnectLoad(demux.Outputs[i][0]);
                mux.ConnectInput(i, MultiBitRegisters[i].Output);
            }

            wsLoad[0].ConnectInput(Load);
            demux.ConnectInput(wsLoad);
            demux.ConnectControl(Address);
            mux.ConnectControl(Address);
            Output.ConnectInput(mux.Output);
        }

        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        public void ConnectAddress(WireSet wsAddress)
        {
            Address.ConnectInput(wsAddress);
        }


        public override void OnClockUp()
        {
        }

        public override void OnClockDown()
        {
        }

        public override string ToString()
        {
            String ans = "Load: " + Load + ", Input: " + Input + "\n";
            for (int i = numOfRegisters - 1; i >= 0; i--)
            {
                ans += MultiBitRegisters[i] + "\n";
            }

            ans += "Output: " + Output + "\n";

            return ans;
        }

        public override bool TestGate()
        {
            for (int k = 0; k < 2; k++)
            {
                Load.Value = k;
                for (int i = 0; i < Math.Pow(2, WordSize); i++)
                {
                    Input.SetValue(i);
                    for (int j = 0; j < numOfRegisters; j++)
                    {
                        int lastOut = Output.GetValue();
                        Address.SetValue(j);
                        Clock.ClockDown();
                        Clock.ClockUp();

                        if (Load.Value == 1 &&
                            (Input.GetValue() != MultiBitRegisters[Address.GetValue()].Output.GetValue() ||
                             Input.GetValue() != mux.Output.GetValue()))
                        {
                            return false;
                        }

                        if (Load.Value == 0 && Output.GetValue() != lastOut && Input.GetValue() != lastOut)
                        {
                            return false;
                        }
                    }

                    //Console.WriteLine(this);
                }
            }

            return true;
        }
    }
}