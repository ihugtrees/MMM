using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a mux with k input, each input with n wires. The output also has n wires.

    class BitwiseMultiwayMux : Gate
    {
        //Word size - number of bits in each output
        public int Size { get; private set; }

        //The number of control bits needed for k outputs
        public int ControlBits { get; private set; }

        public WireSet Output { get; private set; }
        public WireSet Control { get; private set; }
        public WireSet[] Inputs { get; private set; }

        private BitwiseMux[] muxes;

        public BitwiseMultiwayMux(int iSize, int cControlBits)
        {
            int numOfInputs = (int) Math.Pow(2, cControlBits);

            ControlBits = cControlBits;
            Size = iSize;
            Output = new WireSet(Size);
            Control = new WireSet(cControlBits);
            Inputs = new WireSet[numOfInputs];
            muxes = new BitwiseMux[numOfInputs - 1];

            for (int i = 0; i < numOfInputs - 1; i++)
            {
                Inputs[i] = new WireSet(Size);
                muxes[i] = new BitwiseMux(Size);
            }
            Inputs[numOfInputs - 1] = new WireSet(Size);

            for (int i = 0; i < ControlBits - 1; i++)
            {
                for (int j = (int) Math.Pow(2, i) - 1; j < Math.Pow(2, i + 1) - 1; j++)
                {
                    muxes[j].ConnectInput1(muxes[j * 2 + 1].Output);
                    muxes[j].ConnectInput2(muxes[j * 2 + 2].Output);
                    muxes[j].ConnectControl(Control[ControlBits - 1 - i]);
                }
            }

            int inputIndex = 0;
            for (int i = (int) Math.Pow(2, ControlBits - 1) - 1; i < Math.Pow(2, ControlBits) - 1; i++)
            {
                muxes[i].ConnectInput1(Inputs[inputIndex]);
                inputIndex++;
                muxes[i].ConnectInput2(Inputs[inputIndex]);
                inputIndex++;
                muxes[i].ConnectControl(Control[0]);
            }

            Output.ConnectInput(muxes[0].Output);
        }


        public void ConnectInput(int i, WireSet wsInput)
        {
            Inputs[i].ConnectInput(wsInput);
        }

        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }


        public override bool TestGate()
        {
            Inputs[0].SetValue(3);
            Inputs[1].SetValue(4);
            for (int i = 0; i < Math.Pow(2, ControlBits); i++)
            {
                Control.SetValue(i);
                if (Output.GetValue() != Inputs[i].GetValue())
                    return false;
            }

            return true;
        }
    }
}