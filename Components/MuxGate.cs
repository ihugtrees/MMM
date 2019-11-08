using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A mux has 2 inputs. There is a single output and a control bit, selecting which of the 2 inputs should be directed to the output.
    class MuxGate : TwoInputGate
    {
        public Wire ControlInput { get; private set; }

        private AndGate and1;
        private AndGate and2;
        private NotGate not;
        private OrGate or;


        public MuxGate()
        {
            ControlInput = new Wire();
            
            and1 = new AndGate();
            and2 = new AndGate();
            not = new NotGate();
            or = new OrGate();

            and1.ConnectInput1(Input1);
            not.ConnectInput(ControlInput);
            and1.ConnectInput2(not.Output);

            and2.ConnectInput1(Input2);
            and2.ConnectInput2(ControlInput);
            
            or.ConnectInput1(and1.Output);
            or.ConnectInput2(and2.Output);

            Output = or.Output;
        }

        public void ConnectControl(Wire wControl)
        {
            ControlInput.ConnectInput(wControl);
        }


        public override string ToString()
        {
            return "Mux " + Input1.Value + "," + Input2.Value + ",C" + ControlInput.Value + " -> " + Output.Value;
        }


        public override bool TestGate()
        {
            ControlInput.Value = 0;
            for (int i = 0; i < 4; i++)
            {
                int theValue = i;
                Input1.Value = theValue % 2;
                Input2.Value = theValue / 2;

                if (Output.Value != Input1.Value)
                {
                    return false;
                }
            }

            ControlInput.Value = 1;
            for (int i = 0; i < 4; i++)
            {
                int theValue = i;
                Input1.Value = theValue % 2;
                Input2.Value = theValue / 2;

                if (Output.Value != Input2.Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}