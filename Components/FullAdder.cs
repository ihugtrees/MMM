using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a FullAdder, taking as input 3 bits - 2 numbers and a carry, and computing the result in the output, and the carry out.
    class FullAdder : TwoInputGate
    {
        public Wire CarryInput { get; private set; }
        public Wire CarryOutput { get; private set; }

        private HalfAdder inputAdder;
        private HalfAdder outputAdder;
        private OrGate or;


        public FullAdder()
        {
            CarryInput = new Wire();
            CarryOutput = new Wire();
            inputAdder = new HalfAdder();
            outputAdder = new HalfAdder();
            or = new OrGate();

            inputAdder.ConnectInput1(Input1);
            inputAdder.ConnectInput2(Input2);
            outputAdder.ConnectInput1(inputAdder.Output);
            outputAdder.ConnectInput2(CarryInput);
            or.ConnectInput1(inputAdder.CarryOutput);
            or.ConnectInput2(outputAdder.CarryOutput);
            CarryOutput.ConnectInput(or.Output);
            Output.ConnectInput(outputAdder.Output);
        }


        public override string ToString()
        {
            return Input1.Value + "+" + Input2.Value + " (C" + CarryInput.Value + ") = " + Output.Value + " (C" +
                   CarryOutput.Value + ")";
        }

        public override bool TestGate()
        {
            for (int i = 0; i < 2; i++)
            {
                CarryInput.Value = i;
                for (int j = 0; j < 2; j++)
                {
                    Input1.Value = j;
                    for (int k = 0; k < 2; k++)
                    {
                        Input2.Value = k;
                        if (i + j + k == 0)
                            if (Output.Value != 0 && CarryOutput.Value != 0)
                                return false;

                        if (i + j + k == 1)
                            if (Output.Value != 1 && CarryOutput.Value != 0)
                                return false;

                        if (i + j + k == 2)
                            if (Output.Value != 0 && CarryOutput.Value != 1)
                                return false;

                        if (i + j + k == 3)
                            if (Output.Value != 1 && CarryOutput.Value != 1)
                                return false;
                    }
                }
            }

            return true;
        }
    }
}