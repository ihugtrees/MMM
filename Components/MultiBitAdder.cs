using System;

namespace Components
{
    //This class implements an adder, receving as input two n bit numbers, and outputing the sum of the two numbers
    class MultiBitAdder : Gate
    {
        //Word size - number of bits in each input
        public int Size { get; private set; }

        public WireSet Input1 { get; private set; }
        public WireSet Input2 { get; private set; }

        public WireSet Output { get; private set; }

        //An overflow bit for the summation computation
        public Wire Overflow { get; private set; }

        private HalfAdder HalfAdder;
        private FullAdder[] FullAdders;


        public MultiBitAdder(int iSize)
        {
            Size = iSize;
            Input1 = new WireSet(Size);
            Input2 = new WireSet(Size);
            Output = new WireSet(Size);
            Overflow = new Wire();
            HalfAdder = new HalfAdder();
            FullAdders = new FullAdder[Size - 1];

            for (int i = 0; i < Size - 1; i++)
            {
                FullAdders[i] = new FullAdder();
                FullAdders[i].ConnectInput1(Input1[i + 1]);
                FullAdders[i].ConnectInput2(Input2[i + 1]);
                Output[i + 1].ConnectInput(FullAdders[i].Output);
                if (i > 0)
                    FullAdders[i].CarryInput.ConnectInput(FullAdders[i - 1].CarryOutput);
            }

            Overflow.ConnectInput(FullAdders[FullAdders.Length - 1].CarryOutput);
            HalfAdder.ConnectInput1(Input1[0]);
            HalfAdder.ConnectInput2(Input2[0]);
            FullAdders[0].CarryInput.ConnectInput(HalfAdder.CarryOutput);
            Output[0].ConnectInput(HalfAdder.Output);
        }

        public override string ToString()
        {
            return Input1 + "(" + Input1.Get2sComplement() + ")" + " + " + Input2 + "(" + Input2.Get2sComplement() +
                   ")" + " = " + Output + "(" + Output.Get2sComplement() + ")" + " Overflow: " + Overflow;
        }

        public void ConnectInput1(WireSet wInput)
        {
            Input1.ConnectInput(wInput);
        }

        public void ConnectInput2(WireSet wInput)
        {
            Input2.ConnectInput(wInput);
        }


        public override bool TestGate()
        {
            int maxVal = (int) Math.Pow(2, Size - 1);
            int minVal = (maxVal - 1) * -1;
            int counter = 0;
            
            
            for (int i = minVal; i < 0; i++)
            {
                Input1.Set2sComplement(i);
                for (int j = counter; j < maxVal; j++)
                {
                    Input2.Set2sComplement(j);
                    //Console.WriteLine(this);
                    if ((Input1.Get2sComplement() + Input2.Get2sComplement()) != Output.Get2sComplement())
                        return false;
                }

                counter--;
            }

            counter = maxVal;
            for (int i = 0; i < maxVal; i++)
            {
                Input1.Set2sComplement(i);
                for (int j = minVal; j < counter; j++)
                {
                    Input2.Set2sComplement(j);
                    //Console.WriteLine(this);
                    if ((Input1.Get2sComplement() + Input2.Get2sComplement()) != Output.Get2sComplement())
                        return false;
                }

                counter--;
            }

            return true;
        }
    }
}