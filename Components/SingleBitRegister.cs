using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a register that can maintain 1 bit.
    class SingleBitRegister : Gate
    {
        public Wire Input { get; private set; }

        public Wire Output { get; private set; }

        //A bit setting the register operation to read or write
        public Wire Load { get; private set; }

        private MuxGate Mux;
        private DFlipFlopGate FlipFlopGate;

        public SingleBitRegister()
        {
            Input = new Wire();
            Load = new Wire();
            Output = new Wire();
            Mux = new MuxGate();
            FlipFlopGate = new DFlipFlopGate();

            Mux.ConnectControl(Load);
            Mux.ConnectInput1(FlipFlopGate.Output);
            Mux.ConnectInput2(Input);
            FlipFlopGate.ConnectInput(Mux.Output);
            Output.ConnectInput(FlipFlopGate.Output);
        }

        public void ConnectInput(Wire wInput)
        {
            Input.ConnectInput(wInput);
        }

        public void ConnectLoad(Wire wLoad)
        {
            Load.ConnectInput(wLoad);
        }

        public override bool TestGate()
        {
            for (int i = 0; i < 2; i++)
            {
                Load.Value = i;
                for (int j = 0; j < 2; j++)
                {
                    Input.Value = j;
                    Clock.ClockDown();
                    Clock.ClockUp();

                    if (Load.Value == 1 && Output.Value != Input.Value)
                    {
                        return false;
                    }

                    if (Load.Value == 0 && FlipFlopGate.Output.Value != Input.Value &&
                        Output.Value != FlipFlopGate.Output.Value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}