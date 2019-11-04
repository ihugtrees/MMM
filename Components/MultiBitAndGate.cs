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
                int inputIndex = m_wsInput.Size - 1;
                int inputValue = i;

                while (inputValue != 0)
                {
                    m_wsInput[inputIndex].Value = inputValue % 2;
                    inputValue /= 2;
                    inputIndex--;
                }

                for (int j = 0; j < m_wsInput.Size; j++)
                    if (m_wsInput[j].Value == 0)
                        if (Output.Value != 0)
                            return false;
            }

            return true;
        }
    }
}