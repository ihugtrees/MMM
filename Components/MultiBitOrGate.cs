using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //Multibit gates take as input k bits, and compute a function over all bits - z=f(x_0,x_1,...,x_k)

    class MultiBitOrGate : MultiBitGate
    {
        //your code here

        public MultiBitOrGate(int iInputCount)
            : base(iInputCount)
        {
            //your code here

        }

        public override bool TestGate()
        {
            for (var i = 0; i < m_wsInput.Size; i++)
                if (m_wsInput[i].Value == 1)
                    return true;
            return false;
        }
    }
}
