using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This gate implements the or operation. To implement it, follow the example in the And gate.
    class OrGate : TwoInputGate
    {
        //we will use a * and a * after it
        private NotGate m_gNot;
        private NAndGate m_gNand;

        public OrGate()
        {
            //init the gates
            m_gNand = new NAndGate();
            m_gNot = new NotGate();
            //wire the output of the nand gate to the input of the not
            m_gNot.ConnectInput(m_gNand.Output);
            //set the inputs and the output of the and gate
            Output = m_gNot.Output;
            Input1 = m_gNand.Input1;
            Input2 = m_gNand.Input2;
        }


        public override string ToString()
        {
            return "Or " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }

        public override bool TestGate()
        {
            Input1.Value = 0;
            Input2.Value = 0;
            if (Output.Value != 0)
                return false;
            Input1.Value = 0;
            Input2.Value = 1;
            if (Output.Value != 1)
                return false;
            Input1.Value = 1;
            Input2.Value = 0;
            if (Output.Value != 1)
                return false;
            Input1.Value = 1;
            Input2.Value = 1;
            if (Output.Value != 1)
                return false;
            return true;
        }
    }

}
