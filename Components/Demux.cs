using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A demux has 2 outputs. There is a single input and a control bit, selecting whether the input should be directed to the first or second output.
    class Demux : Gate
    {
        public Wire Output1 { get; private set; }
        public Wire Output2 { get; private set; }
        public Wire Input { get; private set; }
        public Wire Control { get; private set; }

        private AndGate and1;
        private AndGate and2;
        private NotGate not;

        public Demux()
        {
            Input = new Wire();
            Output1 = new Wire();
            Output2 = new Wire();
            Control = new Wire();

            and1 = new AndGate();
            and2 = new AndGate();
            not = new NotGate();

            and1.ConnectInput1(Input);
            not.ConnectInput(Control);
            and1.ConnectInput2(not.Output);

            and2.ConnectInput1(Input);
            and2.ConnectInput2(Control);

            Output1 = and1.Output;
            Output2 = and2.Output;
        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }

        public void ConnectInput(Wire wInput)
        {
            Input.ConnectInput(wInput);
        }

        public override bool TestGate()
        {
            Control.Value = 0;

            Input.Value = 0;
            if (Output1.Value != 0 || Output2.Value != 0)
                return false;

            Input.Value = 1;
            if (Output1.Value != 1 || Output2.Value != 0)
                return false;

            Control.Value = 1;

            Input.Value = 0;
            if (Output1.Value != 0 || Output2.Value != 0)
                return false;

            Input.Value = 1;
            if (Output1.Value != 0 || Output2.Value != 1)
                return false;

            return true;
        }
    }
}