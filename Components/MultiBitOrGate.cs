﻿using System;
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

            m_wsInput[0].ConnectInput(orGates[0].Input1);

            for (var i = 1; i < orGates.Length; i++)
            {
                orGates[i].Input1.ConnectInput(orGates[i - 1].Output);
                m_wsInput[i].ConnectInput(orGates[i - 1].Input2);
            }

            m_wsInput[iInputCount - 1].ConnectInput(orGates[iInputCount - 2].Input2);
            Output = orGates[iInputCount - 2].Output;

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
                    if (m_wsInput[j].Value == 1)
                        if (Output.Value != 1)
                            return false;
            }

            return true;
        }
    }
}
