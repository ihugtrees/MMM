﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This bitwise gate takes as input one WireSet containing n wires, and computes a bitwise function - z_i=f(x_i)
    class BitwiseNotGate : Gate
    {
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        public int Size { get; private set; }

        private NotGate[] nots;

        public BitwiseNotGate(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);

            nots = new NotGate[iSize];
            for (int i = 0; i < iSize; i++)
            {
                nots[i] = new NotGate();
                nots[i].ConnectInput(Input[i]);
                Output[i].ConnectInput(nots[i].Output);
            }
        }

        public void ConnectInput(WireSet ws)
        {
            Input.ConnectInput(ws);
        }

        //an implementation of the ToString method is called, e.g. when we use Console.WriteLine(not)
        //this is very helpful during debugging
        public override string ToString()
        {
            return "Not " + Input + " -> " + Output;
        }

        public override bool TestGate()
        {
            for (int j = 0; j < Math.Pow(2, Input.Size); j++)
            {
                Input.SetValue(j);

                for (int i = 0; i < nots.Length; i++)
                    if (Input[i].Value == Output[i].Value)
                        return false;
            }

            return true;
        }
    }
}