using SME;
using Deflib;
using System;

namespace sum_lastaxis
{

    [ClockedProcess]
    public class SumLastAxis : SimpleProcess
    {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;
        [InputBus]
        private IndexValue index;

        [OutputBus]
        private ValueTransfer m_output;

        private double accumulated;
        private int ind = -1;

        public SumLastAxis(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index, ValueTransfer output)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
        }

        protected override void OnTick()
        {
            if (index.Ready == true)
            {
                if (index.Addr == ind)
                {
                    accumulated += m_input.Data;
                    m_output.value = accumulated;
                }
                else
                {
                    m_output.value = accumulated;
                    accumulated = m_input.Data;
                    ind = index.Addr;
                }
            }
        }
    }

    [ClockedProcess]
    public class SLAIndex : SimpleProcess
    {
        [InputBus]
        private IndexControl controlA;
        [InputBus]
        private IndexControl control;

        [OutputBus]
        private IndexValue outputA;
        [OutputBus]
        private IndexValue outputB;
        [OutputBus]
        private IndexControl controlout;

        private bool running = false;
        private int i = 0, j = 0;
        private int Addr;
        private int width, height;
        private bool Aready, Bready = false;
        private bool started = false;

        public SLAIndex(IndexControl controlA, IndexValue outputA, IndexValue outputB, IndexControl controlout, IndexControl control)
        {
            this.controlA = controlA;
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

                outputA.Addr = i*width + j;
                outputB.Addr = i;

                j++;

                if (j >= width)
                {
                    j = 0;
                    i++;
                }
                if (i >= height)
                {
                    running = false;
                }
            }
            else
            {
                Aready |= controlA.Ready;

                if (Aready)
                {
                    Aready = false;

                    running = true;
                    width = control.Width;
                    height = control.Height;

                    i = 0;
                    outputA.Ready = false;
                    outputA.Addr = controlA.OffsetA;
                    outputB.Ready = false;
                    outputB.Addr = controlA.OffsetB;
                    started = true;
                }

                else
                {
                    if (started == true)
                    {
                        controlout.Ready = true;
                        controlout.Height = control.Height;
                        controlout.Width = control.Width;
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