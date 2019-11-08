using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A bitwise gate takes as input WireSets containing n wires, and computes a bitwise function - z_i=f(x_i)
    class BitwiseMux : BitwiseTwoInputGate
    {
        public Wire ControlInput { get; private set; }

        private MuxGate[] muxs;

        public BitwiseMux(int iSize)
            : base(iSize)
        {
            ControlInput = new Wire();

            muxs = new MuxGate[iSize];
            for (int i = 0; i < iSize; i++)
                muxs[i] = new MuxGate();
        }

        public void ConnectControl(Wire wControl)
        {
            ControlInput.ConnectInput(wControl);
        }


        public override string ToString()
        {
            return "Mux " + Input1 + "," + Input2 + ",C" + ControlInput.Value + " -> " + Output;
        }


        public override bool TestGate()
        {
            ControlInput.Value = 0;
            for (int i = 0; i < Input1.Size; i++)
            {
                if (Output[i].Value != Input1[i].Value)
                    return false;
            }
            
            ControlInput.Value = 1;
            for (int i = 0; i < Input1.Size; i++)
            {
                if (Output[i].Value != Input2[i].Value)
                    return false;
            }

            return true;
        }
    }
}