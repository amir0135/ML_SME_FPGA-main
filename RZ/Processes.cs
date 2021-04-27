using SME;
using Deflib;

namespace RZ
{

    public class ZIndex : SimpleProcess
    {
        [InputBus]
        private IndexControl controlA;
        [InputBus]
        private IndexControl controlB;
        [InputBus]
        private IndexControl control;

        [OutputBus]
        private IndexValue outputA;
        [OutputBus]
        private IndexValue outputB;
        [OutputBus]
        private IndexControl controlout;

        private bool running = false;
        private int i, j, k = 0;
        private int Addr;
        private int width, height, dim;
        private bool Aready = false, Bready = false;
        private bool started = false;

        public ZIndex(IndexControl controlA, IndexControl controlB, IndexValue outputA, IndexValue outputB, IndexControl controlout, IndexControl control)
        {
            this.controlA = controlA;
            this.controlB = controlB;
            this.controlout = controlout;
            this.outputA = outputA;
            this.outputB = outputB;
            this.control = control;
        }

        protected override void OnTick()
        {
            if (running == true)
            {
                outputA.Ready = true;
                outputB.Ready = true;
                started = true;

                outputA.Addr = i * width * height + j * width + k;
                outputB.Addr = i * width * height + j * width + k;

                k++;

                if (k >= width)
                {
                    k = 0;
                    j++;
                }

                if (j >= height)
                {
                    j = 0;
                    i++;
                }

                if (i >= dim)
                {
                    running = false;
                }
            }
            else
            {
                Aready |= controlA.Ready;
                Bready |= controlB.Ready;

                if (Aready && Bready)
                {
                    Aready = false;
                    Bready = false;

                    running = true;
                    width = control.Width;
                    height = control.Height;
                    dim = control.Dim;

                    i = j = k = 0;

                    outputA.Ready = true;
                    outputA.Addr = controlA.OffsetA;
                    outputB.Ready = true;
                    outputB.Addr = controlB.OffsetB;

                    started = true;
                }
                else
                {
                    if (started == true)
                    {
                        controlout.Ready = true;
                        controlout.Height = control.Height;
                        controlout.Width = control.Width;
                        controlout.Dim = control.Dim;
                        controlout.OffsetA = controlA.OffsetA;
                        controlout.OffsetB = controlA.OffsetB;
                        controlout.OffsetC = controlA.OffsetC;
                        started = false;
                    }
                    else
                    {
                        controlout.Ready = false;
                    }

                    outputA.Ready = false;
                    outputB.Ready = false;
                }
            }
        }
    }

}