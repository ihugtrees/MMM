using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A bitwise gate takes as input WireSets containing n wires, and computes a bitwise function - z_i=f(x_i)
    class BitwiseDemux : Gate
    {
        public int Size { get; private set; }
        public WireSet Output1 { get; private set; }
        public WireSet Output2 { get; private set; }
        public WireSet Input { get; private set; }
        public Wire Control { get; private set; }

        private Demux[] demuxs;

        public BitwiseDemux(int iSize)
        {
            Size = iSize;
            Control = new Wire();
            Input = new WireSet(Size);
            Output1 = new WireSet(iSize);
            Output2 = new WireSet(iSize);

            demuxs = new Demux[iSize];
            for (int i = 0; i < iSize; i++)
            {
                demuxs[i] = new Demux();
                demuxs[i].ConnectInput(Input[i]);
                demuxs[i].ConnectControl(Control);
                Output1[i].ConnectInput(demuxs[i].Output1);
                Output2[i].ConnectInput(demuxs[i].Output2);
            }
        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }

        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        public override bool TestGate()
        {
            for (int i = 0; i < Math.Pow(2, Input.Size); i++)
            {
                Input.SetValue(i);

                Control.Value = 0;

                for (int k = 0; k < Input.Size; k++)
                    if (Output1[k].Value != Input[k].Value)
                        return false;

                Control.Value = 1;

                for (int k = 0; k < Input.Size; k++)
                    if (Output2[k].Value != Input[k].Value)
                        return false;
            }

            return true;
        }
    }
}