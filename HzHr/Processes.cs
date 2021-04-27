using System;
using SME;
using Deflib;

namespace HzHr
{

    [ClockedProcess]
    public class Hz : SimpleProcess
    {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<double>.IReadResult m_inputA;
        [InputBus]
        private SME.Components.SimpleDualPortMemory<double>.IReadResult m_inputB;
        [InputBus]
        private IndexValue input_pipe;

        [OutputBus]
        private ValueTransfer v_output;

        public Hz(IndexValue inputpipe, SME.Components.SimpleDualPortMemory<double>.IReadResult inputA, SME.Components.SimpleDualPortMemory<double>.IReadResult inputB, ValueTransfer output)
        {
            input_pipe = inputpipe ?? throw new ArgumentNullException(nameof(inputpipe));
            m_inputA = inputA ?? throw new ArgumentNullException(nameof(inputA));
            m_inputB = inputB ?? throw new ArgumentNullException(nameof(inputB));
            v_output = output ?? throw new ArgumentNullException(nameof(output));
        }

        protected override void OnTick()
        {
            if (input_pipe.Ready == true)
            {
                if (m_inputA.Data < 0)
                {
                    v_output.value = m_inputA.Data * m_inputB.Data;
                }
                else
                {
                    v_output.value = m_inputA.Data;
                }
            }
            else
            {
                v_output.value = 0;
            }
        }
    }

    public class HzIndex : SimpleProcess
    {
        [InputBus]
        private IndexControl control;
        [InputBus]
        private IndexControl controlA;
        [InputBus]
        private IndexControl controlB;

        [OutputBus]
        private IndexValue outputA;
        [OutputBus]
        private IndexValue outputB;
        [OutputBus]
        private IndexControl controlout;

        private bool running = false;
        private int i = 0, j = 0, k = 0;
        private int Addr;
        private int width, height, dim;
        private bool Aready = false, Bready = false;
        private bool started = false;

        public HzIndex(IndexControl controlA, IndexControl controlB, IndexValue outputA, IndexValue outputB, IndexControl controlout, IndexControl control)
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

                outputA.Addr = i*width*height + j*width + k;
                outputB.Addr = j;

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

