using SME;
using Deflib;
using System;

namespace Softplus
{
    [ClockedProcess]
    public class Softplus_1: SimpleProcess {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;

        [InputBus]
        private IndexValue index;
  
        [OutputBus]
        private ValueTransfer m_output;

        private int threshold = 20;


        public Softplus_1(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index,  ValueTransfer output)
        {   
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
        }

        protected override void OnTick(){
            if (index.Ready == true){
                m_output.value = Math.Exp(m_input.Data);
                    
            }
        }  
    }

        [ClockedProcess]
    public class Softplus_2: SimpleProcess {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;

        [InputBus]
        private IndexValue index;
  
        [OutputBus]
        private ValueTransfer m_output;


        public Softplus_2(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index,  ValueTransfer output)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
        }

        protected override void OnTick(){
    
            if (index.Ready == true){
                m_output.value = Math.Log(1 + m_input.Data);
                    
            }
        }  
    }
}