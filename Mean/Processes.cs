using SME;
using Deflib;
using System;

namespace Mean
{

    [ClockedProcess]
    public class Mean_count : SimpleProcess
    {
        [InputBus]
        private IndexValue index;
        [InputBus]
        private Flag flush;

        [OutputBus]
        private ValueTransfer m_output;

        private int count = 0;

        public Mean_count(IndexValue index, ValueTransfer output, Flag flush)
        {
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            this.m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.flush = flush ?? throw new ArgumentNullException(nameof(flush));
        }

        protected override void OnTick()
        {
            if (index.Ready)
            {
                if (flush.flg)
                {
                    m_output.value = count + 1;
                    count = 0;
                }
                else
                {
                    count += 1;
                }
            }
        }
    }

    [ClockedProcess]
    public class Mean_add : SimpleProcess
    {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;
        [InputBus]
        private IndexValue index;
        [InputBus]
        private Flag flush;

        [OutputBus]
        private ValueTransfer m_output;

        private double accumulated = 0;

        public Mean_add(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index, ValueTransfer output, Flag flush)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.flush = flush ?? throw new ArgumentNullException(nameof(flush));
        }

        protected override void OnTick()
        {
            if (index.Ready)
            {
                if (flush.flg)
                {
                    m_output.value = accumulated + m_input.Data;
                    accumulated = 0;
                }
                else
                {
                    accumulated += m_input.Data;
                }
            }
        }
    }


    [ClockedProcess]
    public class Mean_div : SimpleProcess
    {
        [InputBus]
        private ValueTransfer m_input;
        [InputBus]
        private ValueTransfer m_count;
        [InputBus]
        private IndexValue index;
        [InputBus]
        private Flag flush;

        [OutputBus]
        private ValueTransfer m_output;

        private int ind = -1;

        public Mean_div(ValueTransfer input, ValueTransfer count, IndexValue index, Flag flush, ValueTransfer output)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            m_count = count ?? throw new ArgumentNullException(nameof(count));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.flush = flush ?? throw new ArgumentNullException(nameof(flush));
        }

        protected override void OnTick()
        {
            if (index.Ready && flush.flg)
            {
                m_output.value = m_input.value / m_count.value;
                ind = index.Addr;
            }
        }
    }

    [ClockedProcess]
    public class ShouldSave : SimpleProcess
    {
        [InputBus]
        private IndexValue input;
        [InputBus]
        private Flag flush;

        [OutputBus]
        private IndexValue output;

        public ShouldSave(IndexValue input, Flag flush, IndexValue output)
        {
            this.input = input;
            this.flush = flush;
            this.output = output;
        }

        protected override void OnTick()
        {
            output.Ready = input.Ready && flush.flg;
            if (input.Ready)
            {
                output.Addr = input.Addr;
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
        [OutputBus]
        private Flag flush;

        private bool running = false;
        private int i = 0, j = 0;
        private int width, height;
        private bool Aready = false;
        private bool started = false;

        public SLAIndex(IndexControl controlA, IndexValue outputA, IndexValue outputB, IndexControl controlout, IndexControl control, Flag flush)
        {
            this.controlA = controlA;
            this.controlout = controlout;
            this.outputA = outputA;
            this.outputB = outputB;
            this.control = control;
            this.flush = flush;
        }

        protected override void OnTick()
        {
            flush.flg = false;
            if (running)
            {
                outputA.Ready = true;
                outputB.Ready = true;
                started = true;

                outputA.Addr = i * width + j;
                outputB.Addr = i;

                j++;

                if (j >= width)
                {
                    j = 0;
                    i++;
                    flush.flg = true;
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
                    j = 0;
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
                        controlout.OffsetC = controlA.OffsetC;
                        controlout.Dim = controlA.Dim;
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