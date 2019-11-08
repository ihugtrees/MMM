﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A two input bitwise gate takes as input two WireSets containing n wires, and computes a bitwise function - z_i=f(x_i,y_i)
    class BitwiseAndGate : BitwiseTwoInputGate
    {
        private AndGate[] ands;

        public BitwiseAndGate(int iSize)
            : base(iSize)
        {
            ands = new AndGate[iSize];
            for (int i = 0; i < iSize; i++)
            {
                ands[i] = new AndGate();
                ands[i].ConnectInput1(Input1[i]);
                ands[i].ConnectInput2(Input2[i]);

                Output[i].ConnectInput(ands[i].Output);
            }
        }

        //an implementation of the ToString method is called, e.g. when we use Console.WriteLine(and)
        //this is very helpful during debugging
        public override string ToString()
        {
            return "And " + Input1 + ", " + Input2 + " -> " + Output;
        }

        public override bool TestGate()
        {
            for (int i = 0; i < Math.Pow(2, Input1.Size); i++)
            {
                Input1.SetValue(i);

                for (int j = 0; j < Math.Pow(2, Input1.Size); j++)
                {
                    Input2.SetValue(j);

                    for (int k = 0; k < Input1.Size; k++)
                    {
                        if (Input1[k].Value == 1 && Input2[k].Value == 1 && Output[k].Value != 1)
                            return false;

                        if ((Input1[k].Value == 0 || Input2[k].Value == 0) && Output[k].Value != 0)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}