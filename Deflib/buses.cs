using SME;
using System;

namespace Deflib
{

    public interface ValueTransfer : IBus
    {
        float value { get; set; }
    }

    public interface IndexTransfer : IBus
    {
        int index { get; set; }
    }

    public interface VectorTransfer : IBus
    {
        float[] vector { get; set; }
    }

    public interface MatrixTransfer : IBus
    {
        float[,] matrix { get; set; }
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
        int Dim { get; set; }
        int OffsetA { get; set; }
        int OffsetB { get; set; }
        int OffsetC { get; set; }
    }

}