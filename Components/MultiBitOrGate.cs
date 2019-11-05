using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //Multibit gates take as input k bits, and compute a function over all bits - z=f(x_0,x_1,...,x_k)

    class MultiBitOrGate : MultiBitGate
    {
        private OrGate[] orGates;

        public MultiBitOrGate(int iInputCount)
            : base(iInputCount)
        {
            orGates = new OrGate [iInputCount - 1];
            for (int i = 0; i < orGates.Length; i++)
                orGates[i] = new OrGate();

            orGates[0].ConnectInput1(m_wsInput[0]);

            for (var i = 1; i < orGates.Length; i++)
            {
                orGates[i].ConnectInput1(orGates[i - 1].Output);
                orGates[i - 1].ConnectInput2(m_wsInput[i]);
            }

            orGates[iInputCount - 2].ConnectInput2(m_wsInput[iInputCount - 1]);
            Output = orGates[iInputCount - 2].Output;
        }

        public override bool TestGate()
        {
            for (var i = 0; i < Math.Pow(2, m_wsInput.Size); i++)
            {
                m_wsInput.SetValue(i);
                foreach (var orGate in orGates)
                    if ((orGate.Input1.Value == 1 || orGate.Input2.Value == 1) && orGate.Output.Value != 1)
                        return false;
            }

            return true;
        }
    }
}