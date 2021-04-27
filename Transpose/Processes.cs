
using SME;
using Deflib;

namespace Transpose
{

    public class Transpose : SimpleProcess
    {
        [InputBus]
        private IndexControl controlA;

        [OutputBus]
        private IndexValue outputA;
        [OutputBus]
        private IndexValue outputB;
        [OutputBus]
        private IndexControl controlout;

        private bool running = false;
        private int i, j = 0;
        private int Addr;
        private int width, height;
        private bool started = false;

        public Transpose(IndexControl controlA, IndexValue outputA, IndexValue outputB, IndexControl controlout)
        {
            this.controlout = controlout;
            this.controlA = controlA;
            this.outputA = outputA;
            this.outputB = outputB;
        }

        protected override void OnTick()
        {
            if (running == true)
            {
                started = true;

                j++;

                if (j >= height)
                {
                    j = 0;
                    i++;
                }

                outputA.Addr = i * height + j;
                outputB.Addr = j * width + i;
                if (i >= width - 1 && j >= height - 1)
                {
                    running = false;
                }
            }
            else
            {
                if (controlA.Ready == true)
                {
                    started = true;
                    running = true;
                    width = controlA.Width;
                    height = controlA.Height;
                    i = j = 0;

                    outputA.Ready = true;
                    outputA.Addr = controlA.OffsetA; ;

                    outputB.Ready = true;
                    outputB.Addr = controlA.OffsetB;
                }
                else
                {
                    if (started == true)
                    {
                        controlout.Ready = true;
                        controlout.Height = controlA.Height;
                        controlout.Width = controlA.Width;
                        controlout.OffsetA = controlA.OffsetA;
                        controlout.OffsetB = controlA.OffsetB;
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