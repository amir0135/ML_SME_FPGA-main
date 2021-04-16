using SME;
using Deflib;
using System;

namespace Sigmoid
{

    [ClockedProcess]
    public class Sigmoid_1: SimpleProcess {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;

        [InputBus]
        private IndexValue index;

        [InputBus]
        private Flag flush;
  
        [OutputBus]
        private ValueTransfer m_output;


        public Sigmoid_1(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index,  ValueTransfer output, Flag flush)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.flush = flush ?? throw new ArgumentNullException(nameof(flush));
        }

        protected override void OnTick(){
    

            if (index.Ready == true){
                if (flush.flg) {
                    m_output.value = Math.Exp(-(m_input.Data));
                } 
                    
            }
        }  
    }

        [ClockedProcess]
        public class Sigmoid_2: SimpleProcess {
        [InputBus]
        private ValueTransfer m_input;

        [InputBus]
        private IndexValue index;

        [InputBus]
        private Flag flush;
  
        [OutputBus]
        private ValueTransfer m_output;


        public Sigmoid_2(ValueTransfer input, IndexValue index,  ValueTransfer output, Flag flush)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.flush = flush ?? throw new ArgumentNullException(nameof(flush));
        }

        protected override void OnTick(){
    

            if (index.Ready == true){
                if (flush.flg) {
                    m_output.value = 1 + m_input.value;
                }
                    
            }
        }  
    }
   

    [ClockedProcess]
    public class Sigmoid_3: SimpleProcess {
        [InputBus]
        private ValueTransfer m_input;

        [InputBus]
        private IndexValue index;

        [InputBus]
        private Flag flush;
  
        [OutputBus]
        private ValueTransfer m_output;


        public Sigmoid_3(ValueTransfer input, IndexValue index,  ValueTransfer output, Flag flush)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.flush = flush ?? throw new ArgumentNullException(nameof(flush));
        }

        protected override void OnTick(){
    

            if (index.Ready == true){
                if (flush.flg) {
                    m_output.value = 1 /m_input.value;
                }
                    
            }
        }  
    }
   

   



}    

