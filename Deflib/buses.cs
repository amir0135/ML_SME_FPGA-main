using SME;
using System;

namespace Deflib
{

    public interface ValueTransfer : IBus
    {
        double value { get; set; }
    }

    public interface IndexTransfer : IBus
    {
        int index { get; set; }
    }

    public interface VectorTransfer : IBus
    {
        double[] vector { get; set; }
    }

    public interface MatrixTransfer : IBus
    {
        double[,] matrix { get; set; }
    }

    public interface IndexValue : IBus
    {
        [InitialValue(false)]
        bool Ready { get; set; }
        int Addr { get; set; }
    }

    public interface Flag : IBus
    {
        [InitialValue(false)]
        bool flg { get; set;}
    }

    public interface IndexControl : IBus
    {
        [InitialValue(false)]
        bool Ready { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        [InitialValue(0)]
        int Dim { get; set; }
        [InitialValue(0)]
        int OffsetA { get; set; }
        [InitialValue(0)]
        int OffsetB { get; set; }
        [InitialValue(0)]
        int OffsetC { get; set; }
    }

}