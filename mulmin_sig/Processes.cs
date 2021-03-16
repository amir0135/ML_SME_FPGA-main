using SME;
using Deflib;
using System;

namespace mulmin_sig
{


    [ClockedProcess]
    public class MulMin_1: SimpleProcess {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;

        [InputBus]
        private IndexValue index;
  
        [OutputBus]
        private ValueTransfer m_output;


        public MulMin_1(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index,  ValueTransfer output)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
        }

        protected override void OnTick(){

            if (index.Ready){
                m_output.value = m_input.Data - 1;

            }
        }  
    }

    [ClockedProcess]
    public class MulMin_2: SimpleProcess {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;

        [InputBus]
        private IndexValue index;
  
        [OutputBus]
        private ValueTransfer m_output;


        public MulMin_2(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index,  ValueTransfer output)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
        }

        protected override void OnTick(){

            if (index.Ready){
                m_output.value = 2* m_input.Data ;

            }
        }  
    }
   
}   





