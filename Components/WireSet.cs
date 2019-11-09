using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class represents a set of n wires (a cable)
    class WireSet
    {
        //Word size - number of bits in the register
        public int Size { get; private set; }

        public bool InputConected { get; private set; }

        //An indexer providing access to a single wire in the wireset
        public Wire this[int i]
        {
            get { return m_aWires[i]; }
        }

        private Wire[] m_aWires;

        public WireSet(int iSize)
        {
            Size = iSize;
            InputConected = false;
            m_aWires = new Wire[iSize];
            for (int i = 0; i < m_aWires.Length; i++)
                m_aWires[i] = new Wire();
        }

        public override string ToString()
        {
            string s = "[";
            for (int i = m_aWires.Length - 1; i >= 0; i--)
                s += m_aWires[i].Value;
            s += "]";
            return s;
        }

        //Transform a positive integer value into binary and set the wires accordingly, with 0 being the LSB
        public void SetValue(int iValue)
        {
            int inputIndex = 0;

            while (iValue != 0)
            {
                m_aWires[inputIndex].Value = iValue % 2;
                iValue /= 2;
                inputIndex++;
            }
        }

        //Transform the binary code into a positive integer
        public int GetValue()
        {
            int sum = 0;
            for (int i = 0; i < Size; i++)
            {
                sum = sum + (int) (m_aWires[i].Value * Math.Pow(2, i));
            }

            return sum;
        }

        //Transform an integer value into binary using 2`s complement and set the wires accordingly, with 0 being the LSB
        public void Set2sComplement(int iValue)
        {
            if (iValue < 0)
            {
                SetValue(iValue * (-1));
                reverseNumbers(this);
                add1ToBinary(this);

                m_aWires[Size - 1].Value = 1;
            }
            else
            {
                SetValue(iValue);
                m_aWires[Size - 1].Value = 0;
            }
        }

        //Transform the binary code in 2`s complement into an integer
        public int Get2sComplement()
        {
            WireSet newWs = new WireSet(Size - 1);

            for (int i = 0; i < Size - 1; i++)
                newWs[i].Value = m_aWires[i].Value;

            if (m_aWires[Size - 1].Value == 1)
            {
                reverseNumbers(newWs);
                add1ToBinary(newWs);
            }

            return newWs.GetValue();
        }

        private void reverseNumbers(WireSet wireSet)
        {
            for (int i = 0; i < Size; i++)
            {
                if (wireSet[i].Value == 0)
                    wireSet[i].Value = 1;
                else
                    wireSet[i].Value = 0;
            }
        }

        private void add1ToBinary(WireSet wireSet)
        {
            wireSet[0].Value += 1;
            for (int i = 0; i < wireSet.Size; i++)
            {
                if (wireSet[i].Value == 2)
                {
                    wireSet[i].Value = 0;
                    if (i < wireSet.Size - 1)
                        wireSet[i + 1].Value += 1;
                }
            }
        }

        public void ConnectInput(WireSet wIn)
        {
            if (InputConected)
                throw new InvalidOperationException("Cannot connect a wire to more than one inputs");
            if (wIn.Size != Size)
                throw new InvalidOperationException("Cannot connect two wiresets of different sizes.");
            for (int i = 0; i < m_aWires.Length; i++)
                m_aWires[i].ConnectInput(wIn[i]);

            InputConected = true;
        }
    }
}