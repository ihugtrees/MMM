using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class is used to implement the ALU
    //The specification can be found at https://b1391bd6-da3d-477d-8c01-38cdf774495a.filesusr.com/ugd/56440f_2e6113c60ec34ed0bc2035c9d1313066.pdf slides 48,49
    class ALU : Gate
    {
        //The word size = number of bit in the input and output
        public int Size { get; private set; }

        //Input and output n bit numbers
        public WireSet InputX { get; private set; }
        public WireSet InputY { get; private set; }
        public WireSet Output { get; private set; }

        //Control bit 
        public Wire ZeroX { get; private set; }
        public Wire ZeroY { get; private set; }
        public Wire NotX { get; private set; }
        public Wire NotY { get; private set; }
        public Wire F { get; private set; }
        public Wire NotOutput { get; private set; }

        //Bit outputs
        public Wire Zero { get; private set; }
        public Wire Negative { get; private set; }

        private BitwiseMux zx;
        private BitwiseMux zy;
        private BitwiseMux nx;
        private BitwiseMux ny;
        private BitwiseMux f;
        private BitwiseMux n0;
        private BitwiseNotGate notNx;
        private BitwiseNotGate notNy;
        private BitwiseNotGate notN0;
        private BitwiseNotGate notMultiAnd;
        private BitwiseAndGate NxAndNy;
        private MultiBitAdder NxNyAdder;
        private MultiBitAndGate MultiBitAnd;

        public ALU(int iSize)
        {
            Size = iSize;
            InputX = new WireSet(Size);
            InputY = new WireSet(Size);
            ZeroX = new Wire();
            ZeroY = new Wire();
            NotX = new Wire();
            NotY = new Wire();
            F = new Wire();
            NotOutput = new Wire();
            Negative = new Wire();
            Zero = new Wire();
            Output = new WireSet(Size);

            zx = new BitwiseMux(Size);
            zy = new BitwiseMux(Size);
            nx = new BitwiseMux(Size);
            ny = new BitwiseMux(Size);
            f = new BitwiseMux(Size);
            n0 = new BitwiseMux(Size);
            notNx = new BitwiseNotGate(Size);
            notNy = new BitwiseNotGate(Size);
            notN0 = new BitwiseNotGate(Size);
            notMultiAnd = new BitwiseNotGate(Size);
            NxAndNy = new BitwiseAndGate(Size);
            NxNyAdder = new MultiBitAdder(Size);
            MultiBitAnd = new MultiBitAndGate(Size);

            //zx
            zx.ConnectInput1(InputX);
            zx.ConnectInput2(new WireSet(Size));
            zx.ConnectControl(ZeroX);
            //zy
            zy.ConnectInput1(InputY);
            zy.ConnectInput2(new WireSet(Size));
            zy.ConnectControl(ZeroY);
            //nx
            nx.ConnectInput1(zx.Output);
            notNx.ConnectInput(zx.Output);
            nx.ConnectInput2(notNx.Output);
            nx.ConnectControl(NotX);
            //ny
            ny.ConnectInput1(zy.Output);
            notNy.ConnectInput(zy.Output);
            ny.ConnectInput2(notNy.Output);
            ny.ConnectControl(NotY);
            //nx and ny
            NxAndNy.ConnectInput1(nx.Output);
            NxAndNy.ConnectInput2(ny.Output);
            //multi bit adder
            NxNyAdder.ConnectInput1(nx.Output);
            NxNyAdder.ConnectInput2(ny.Output);
            // f 
            f.ConnectInput1(NxAndNy.Output);
            f.ConnectInput2(NxNyAdder.Output);
            f.ConnectControl(F);
            // n0
            n0.ConnectInput1(f.Output);
            notN0.ConnectInput(f.Output);
            n0.ConnectInput2(notN0.Output);
            n0.ConnectControl(NotOutput);
            //outputs
            notMultiAnd.ConnectInput(n0.Output);
            MultiBitAnd.ConnectInput(notMultiAnd.Output);
            Zero.ConnectInput(MultiBitAnd.Output);
            Output.ConnectInput(n0.Output);
            Negative.ConnectInput(Output[Output.Size - 1]);
        }

        public override bool TestGate()
        {
            //101010 output 0
            ZeroX.Value = 1;
            NotX.Value = 0;
            ZeroY.Value = 1;
            NotY.Value = 0;
            F.Value = 1;
            NotOutput.Value = 0;
            if (Output.Get2sComplement() != 0)
                return false;

            //all 1 output 1
            ZeroX.Value = 1;
            NotX.Value = 1;
            ZeroY.Value = 1;
            NotY.Value = 1;
            F.Value = 1;
            NotOutput.Value = 1;
            if (Output.Get2sComplement() != 1)
                return false;

            //111010 output -1
            ZeroX.Value = 1;
            NotX.Value = 1;
            ZeroY.Value = 1;
            NotY.Value = 0;
            F.Value = 1;
            NotOutput.Value = 0;
            if (Output.Get2sComplement() != -1)
                return false;

            //001100 output x
            ZeroX.Value = 0;
            NotX.Value = 0;
            ZeroY.Value = 1;
            NotY.Value = 1;
            F.Value = 0;
            NotOutput.Value = 0;
            for (int i = 0; i < Size; i++)
                if (Output[i].Value != InputX[i].Value)
                    return false;

            //all 0 output x&y
            ZeroX.Value = 0;
            NotX.Value = 0;
            ZeroY.Value = 0;
            NotY.Value = 0;
            F.Value = 0;
            NotOutput.Value = 0;
            for (int i = 0; i < Size; i++)
                if (Output[i].Value != (InputX[i].Value * InputY[i].Value))
                    return false;

            return true;
        }
    }
}