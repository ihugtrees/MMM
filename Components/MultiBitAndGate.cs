using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //Multibit gates take as input k bits, and compute a function over all bits - z=f(x_0,x_1,...,x_k)
    class MultiBitAndGate : MultiBitGate
    {
        //your code here

        public MultiBitAndGate(int iInputCount)
            : base(iInputCount)
        {
            //your code here

        }


        public override bool TestGate()
        {
            for (var i = 1; i < m_wsInput.Size; i++)
                m_wsInput[i].Value = 1;
            
            m_wsInput[0].Value = 1;
            if (Output.Value != 1)
                return false;
            
            m_wsInput[0].Value = 0;
            if (Output.Value != 0)
                return false;
            
            return true;
        }
    }
}
