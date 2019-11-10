using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a demux with k outputs, each output with n wires. The input also has n wires.

    class BitwiseMultiwayDemux : Gate
    {
        //Word size - number of bits in each output
        public int Size { get; private set; }

        //The number of control bits needed for k outputs
        public int ControlBits { get; private set; }

        public WireSet Input { get; private set; }
        public WireSet Control { get; private set; }
        public WireSet[] Outputs { get; private set; }

        private BitwiseDemux[] demuxes;

        public BitwiseMultiwayDemux(int iSize, int cControlBits)
        {
            int numOfOutputs = (int) Math.Pow(2, cControlBits);

            Size = iSize;
            ControlBits = cControlBits;
            Input = new WireSet(Size);
            Control = new WireSet(cControlBits);
            Outputs = new WireSet[numOfOutputs];
            demuxes = new BitwiseDemux[numOfOutputs - 1];

            for (int i = 0; i < numOfOutputs - 1; i++)
            {
                Outputs[i] = new WireSet(Size);
                demuxes[i] = new BitwiseDemux(Size);
            }

            Outputs[Outputs.Length - 1] = new WireSet(Size);

            for (int i = 0; i < ControlBits - 1; i++)
            {
                for (int j = (int) Math.Pow(2, i) - 1; j < Math.Pow(2, i + 1) - 1; j++)
                {
                    demuxes[j * 2 + 1].ConnectInput(demuxes[j].Output1);
                    demuxes[j * 2 + 2].ConnectInput(demuxes[j].Output2);
                    demuxes[j].ConnectControl(Control[ControlBits - 1 - i]);
                }
            }

            int inputIndex = 0;
            for (int i = (int) Math.Pow(2, ControlBits - 1) - 1; i < Math.Pow(2, ControlBits) - 1; i++)
            {
                Outputs[inputIndex].ConnectInput(demuxes[i].Output1);
                inputIndex++;
                Outputs[inputIndex].ConnectInput(demuxes[i].Output2);
                inputIndex++;
                demuxes[i].ConnectControl(Control[0]);
            }

            demuxes[0].ConnectInput(Input);
        }


        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }


        public override bool TestGate()
        {
            Input.SetValue(3);
            for (int i = 0; i < Math.Pow(2, ControlBits); i++)
            {
                Control.SetValue(i);
                if (Outputs[i].GetValue() != Input.GetValue())
                    return false;
            }

            return true;
        }
    }
}