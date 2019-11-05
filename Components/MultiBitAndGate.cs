using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //Multibit gates take as input k bits, and compute a function over all bits - z=f(x_0,x_1,...,x_k)
    class MultiBitAndGate : MultiBitGate
    {
        private AndGate[] andGates;

        public MultiBitAndGate(int iInputCount)
            : base(iInputCount)
        {
            andGates = new AndGate [iInputCount - 1];
            for (int i = 0; i < andGates.Length; i++)
                andGates[i] = new AndGate();

            andGates[0].ConnectInput1(m_wsInput[0]);

            for (var i = 1; i < andGates.Length; i++)
            {
                andGates[i].ConnectInput1(andGates[i - 1].Output);
                andGates[i - 1].ConnectInput2(m_wsInput[i]);
            }

            andGates[iInputCount - 2].ConnectInput2(m_wsInput[iInputCount - 1]);
            Output = andGates[iInputCount - 2].Output;
        }


        public override bool TestGate()
        {
            for (var i = 0; i < Math.Pow(2, m_wsInput.Size); i++)
            {
                m_wsInput.SetValue(i);
                foreach (var andGate in andGates)
                    if ((andGate.Input1.Value == 0 || andGate.Input2.Value == 0) && andGate.Output.Value != 0)
                        return false;
            }

            return true;
        }
    }
}