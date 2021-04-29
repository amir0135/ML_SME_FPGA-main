using SME;
using Deflib;
using System;

namespace mulmin_sig
{

    [ClockedProcess]
    public class Mulmin_mul : SimpleProcess
    {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<float>.IReadResult m_input;
        [InputBus]
        private IndexValue index;
        [InputBus]
        private Flag flush;

        [OutputBus]
        private ValueTransfer m_output;

        public Mulmin_mul(SME.Components.SimpleDualPortMemory<float>.IReadResult input, IndexValue index, ValueTransfer output, Flag flush)
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
                    m_output.value = 2 * m_input.Data;
                }
            }
        }
    }

    [ClockedProcess]
    public class Mulmin_sub : SimpleProcess
    {
        [InputBus]
        private ValueTransfer m_input;
        [InputBus]
        private IndexValue index;
        [InputBus]
        private Flag flush;

        [OutputBus]
        private ValueTransfer m_output;

        public Mulmin_sub(ValueTransfer input, IndexValue index, ValueTransfer output, Flag flush)
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
                    m_output.value = m_input.value - 1;
                }
            }
        }
    }

}